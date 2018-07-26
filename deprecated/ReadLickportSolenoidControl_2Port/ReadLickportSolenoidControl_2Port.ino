
#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 2x1 megohm resistor between 
                                                          // pins 11 & 10, pin 10 is sensor pin,
                                                          //100 pF capacitor was placed in parallel to increase signal to noise
CapacitiveSensor   R_port = CapacitiveSensor(6, 5);       // 2x1 megohm resistor between pins 6 & 5, pin 5 is sensor pin



int trial_begin = 0; // flag for start of trial
int rflag = 0; // flag to avoid multiple rewards
int r = 0; // tell unity whether to deliver reward (1 = left, 2 = right)

long th = 100; // threshold
long lc = 0; // left lick count
long rc = 0; // right lick count

long start = 0;



int incomingByte = 0;  // cmd coming in from Unity

const int L_pin = 3;
const int R_pin = 4;
const int puff_pin = 2;
const int slider_pin = 8; // slide table
const int led_pin = 9; 
const int ttl0 = 7;  // frame syncing
const int ttl1 = 12; // scan start


const int puff_len = 500; // length of air puff in milliseconds
const int slider_len = 1800; // length of reward availability in milliseconds
const int led_len = 2500; // length of LED on

int L_pin_state = LOW;
int R_pin_state = LOW;
int puff_pin_state = LOW;
int slider_pin_state = LOW;
int led_state = LOW;
int ttl1_state = LOW;



long L_reward_timer = 0;
long R_reward_timer = 0;
long puff_timer = 0;
long slider_timer = 0;
long led_timer = 0;
long ttl1_timer = 0;

int cmd = 0;
int slider_flag = 0;
int led_flag = 0;
int scan_flag = 0;

void setup() {
  Serial.begin(115200);
  //Serial.begin(57600);
  pinMode(puff_pin, OUTPUT);
  pinMode(L_pin,OUTPUT);
  pinMode(R_pin,OUTPUT);
  pinMode(slider_pin,OUTPUT);
  pinMode(led_pin,OUTPUT);
  pinMode(ttl0,OUTPUT);
  pinMode(ttl1,OUTPUT);
  
  digitalWrite(L_pin,LOW);
  digitalWrite(R_pin,LOW);
  digitalWrite(puff_pin,LOW);
  digitalWrite(slider_pin,LOW);
  analogWrite(led_pin,0);
  digitalWrite(ttl0,LOW);
  digitalWrite(ttl1,LOW);
  
  L_port.set_CS_Timeout_Millis(20);
  R_port.set_CS_Timeout_Millis(20);
 
}



void loop() 
{
  
  if ((L_pin_state == HIGH)){
     if (millis()-L_reward_timer>50) {
      digitalWrite(L_pin, LOW);
      L_pin_state = LOW;
     }   
  }

  if ((R_pin_state == HIGH)) { 
    if (millis()-R_reward_timer>25) {
      digitalWrite(R_pin,LOW);
      R_pin_state = LOW;
    }
  }

  if (puff_pin_state == HIGH) {
    if (millis() - puff_timer>puff_len) {
      digitalWrite(puff_pin,LOW);
      puff_pin_state = LOW;
    }
  }

  if (slider_pin_state == HIGH) {
    if (millis() - slider_timer > slider_len) {
      digitalWrite(slider_pin,LOW);
      slider_pin_state = LOW;
    }
  }

  if (led_state == HIGH) {
    if (millis() - led_timer>led_len) {
      analogWrite(led_pin,0);
      led_state = LOW;
      led_flag = 0;
    }
  }

//  if (ttl1_state == HIGH) {
//    if (millis() - ttl1_timer>50) {
//      digitalWrite(ttl1_state,LOW);
//      ttl1_state=LOW;
//    }
//  }

  digitalWrite(ttl0,LOW); // make sure ttl0 is down
  
  long L_val =  L_port.capacitiveSensor(30);
  long R_val =  R_port.capacitiveSensor(30);
   
  if ((L_val >= th) | (L_val==-2)) { // count licks since last Unity frame
    lc++;
    
  }
  if ((R_val >= th) | (R_val==-2)) { 
    rc++;
    
  }

  
  switch (cmd) {

    case 0: // just count licks 
      break;

    case 1: // left reward 
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (rflag == 0)
      { // if reward not dispensed
        if (lc > 0)  { // if first lick is left
          rflag = 1;
          L_pin_state = HIGH;
          digitalWrite(L_pin,HIGH);
          L_reward_timer = millis();
          r=1; // reward left
        }
        if (rc > 0 & lc < 1) 
        { 
          rflag = 1; // if first lick is right prevent reward - r=4 is incorrect right
          r=4; 
          digitalWrite(slider_pin,LOW);
          slider_pin_state=LOW;
          puff_pin_state = HIGH;
          digitalWrite(puff_pin,HIGH);
          puff_timer = millis();
        } 
      }    
      break;
    
    case 2: // right reward
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
    
      if (rflag == 0)
      { // if reward not dispensed
        if (rc > 0) { // if first lick is right
          rflag = 1;
          r = 2; // reward right
          digitalWrite(R_pin,HIGH);
          R_pin_state = HIGH;
          
          R_reward_timer = millis();
         
        }
        if (lc > 0 & rc < 1) 
        { 
          rflag = 1;
          r = 3; // incorrect left
          digitalWrite(slider_pin,LOW);
          slider_pin_state=LOW;
          puff_pin_state = HIGH;
          digitalWrite(puff_pin,HIGH);
          puff_timer = millis();
        } // if  first lick is left prevent reward - r=3 is incorrect left
      } 
      break; 

    case 3: // reset flags
      rflag = 0;
      slider_flag = 0;
      break;

    case 4: // reward double lickport licks that are more than 3 sec apart
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
      
      if (lc>0 & rflag == 0) { // if lick left
        rflag  = 1;
        r = 1;
        L_pin_state = HIGH;
        digitalWrite(L_pin,HIGH);
        L_reward_timer = millis();
        start = millis();
      } else if ( rc>0 & rflag == 0) { // if lick right
        rflag = 1;
        r = 2;
        R_pin_state = HIGH;
        digitalWrite(R_pin,HIGH);
        R_reward_timer = millis();
        start = millis();
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

    case 5: // reward a lick left even if its not first

      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
      
      if (rflag == 0)
      { // if reward not dispensed
        if (lc >0)  { // if any lick is left
         rflag  = 1;
         r = 1;
         L_pin_state = HIGH;
         digitalWrite(L_pin,HIGH);
         L_reward_timer = millis();
         
        }
        
      }
          
      break;

    case 6: // reward a lick right even if its not first
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
      
      if (rflag == 0)
      { // if past 50 milliseconds, no licks yet, and reward not dispensed
        if (rc > 0 )  { // if any lick is right
          rflag = 1;
          r =  2; // reward left
          R_pin_state = HIGH;
          digitalWrite(R_pin,HIGH);
          R_reward_timer = millis();
        }
       
      }
      break;

   case 7: // auto reward left
      L_pin_state = HIGH;
      digitalWrite(L_pin,HIGH);
      L_reward_timer = millis();
      break;

   case 8: // auto reward right
      R_pin_state = HIGH;
      digitalWrite(R_pin,HIGH);
      R_reward_timer = millis();
      break;

   case 9: // cleaning/loading lickports
      digitalWrite(L_pin,HIGH);
      digitalWrite(R_pin,HIGH);
      break;

   case 10: //  turn solenoids off 
      digitalWrite(R_pin,LOW);
      digitalWrite(L_pin,LOW);
      break;

   case 11: // turn LED on
      if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_state=HIGH;
        led_timer = millis();
      }
      break;

   case 12: // toggle microscope on or off:
      digitalWrite(ttl1,HIGH);
      digitalWrite(ttl1,LOW);
      break;

      
   case 13: // start collecting ttl0's for syncing
      scan_flag = 1;
      break;

   case 14: // cmd 11 and 4 
    if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_state=HIGH;
        led_timer = millis();
      }

      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
      
      if (lc>0 & rflag == 0) { // if lick left
        rflag  = 1;
        r = 1;
        L_pin_state = HIGH;
        digitalWrite(L_pin,HIGH);
        L_reward_timer = millis();
        start = millis();
      } else if ( rc>0 & rflag == 0) { // if lick right
        rflag = 1;
        r = 2;
        R_pin_state = HIGH;
        digitalWrite(R_pin,HIGH);
        R_reward_timer = millis();
        start = millis();
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

    case 15: // move port forward and just count licks
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }

      if (rflag == 0)
      { // if reward not dispensed
        if ((lc >0)| (rc>0))  { // if any lick
          r=9;
          rflag =1;
        }
      }
      break;
    
  }

  if (Serial.available()>0) { // if new Unity frame
   
    //cmd = Serial.read()-'0';
    cmd = Serial.parseInt();    //while (Serial.available()) {
    //  cmd += Serial.parseInt();  
    //}

    if (scan_flag>0) { // if scanning
      digitalWrite(ttl0,HIGH); // send frame syncing ttl to scanbox
    }
    
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(rc);                  // print sensor output 2
    Serial.print("\t");
    Serial.print(r);              // print what the reward should was
    Serial.println("");
    lc = 0;
    rc = 0;
    r = 0;
  }
}








