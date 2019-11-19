
#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 2x1 megohm resistor between 
                                                          // pins 11 & 10, pin 10 is sensor pin,
                                                          //100 pF capacitor was placed in parallel to increase signal to noise

int trial_begin = 0; // flag for start of trial
int rflag = 0; // flag to avoid multiple rewards
int r = 0; // tell unity whether to deliver reward (1 = left, 2 = right)

long th = 500; // threshold
long lc = 0; // lick count


long start = 0;



int incomingByte = 0;  // cmd coming in from Unity

const int L_pin = 2;
const int slider_pin = 7; // slide table
const int led_pin = 9;
const int puff_pin = 8; 
const int ttl0 = 5;  // frame syncing
const int ttl1 = 12; // scan start

const int slider_len = 2000; // length of reward availability in milliseconds
const int led_len = 2500; // length of LED on
const int puff_len = 500; // length of airpuff

int L_pin_state = LOW;
int slider_pin_state = LOW;
int led_state = LOW;
int puff_state = LOW;
//int ttl1_state = LOW;



long L_reward_timer = 0;
long slider_timer = 0;
long led_timer = 0;
long ttl1_timer = 0;
long puff_timer = 0;

int cmd = 0;
int slider_flag = 0;
int led_flag = 0;
int scan_flag = 0;
int puff_flag = 0;

void setup() {
  //Serial.begin(115200);
  Serial.begin(57600);
  pinMode(L_pin,OUTPUT);
  pinMode(slider_pin,OUTPUT);
  pinMode(led_pin,OUTPUT);
  pinMode(puff_pin,OUTPUT);
  pinMode(ttl0,OUTPUT);
  pinMode(ttl1,OUTPUT);
  
  digitalWrite(L_pin,LOW);
  digitalWrite(slider_pin,LOW);
  analogWrite(led_pin,0);
  digitalWrite(puff_pin,LOW);
  digitalWrite(ttl0,LOW);
  digitalWrite(ttl1,LOW);
  
  L_port.set_CS_Timeout_Millis(20);
}



void loop() 
{
  
  if ((L_pin_state == HIGH)){
     if (millis()-L_reward_timer>100) {
      digitalWrite(L_pin, LOW);
      L_pin_state = LOW;
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

  if (puff_state==HIGH) {
    if (millis() - puff_timer>puff_len) {
      digitalWrite(puff_pin,LOW);
      puff_state = LOW;
      puff_flag = 0;
    }
  }


  digitalWrite(ttl0,LOW); // make sure ttl0 is down
  
  long L_val =  L_port.capacitiveSensor(50);
  
   
  if ((L_val >= th) | (L_val==-2)) { // count licks since last Unity frame
    lc++;
    
  }
  

  
  switch (cmd) {
    case 8: // toggle microscope on or off:
          
      digitalWrite(ttl1,HIGH);
      digitalWrite(ttl1,LOW);
      scan_flag=0;
          //ttl1_state = HIGH;
         // ttl1_timer = millis();
      break;

    case 0: // just count licks 
      break;

    
    case 1: // reward 
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
      }    
      break;
    
    
    case 2: // reset flags
      rflag = 0;
      slider_flag = 0;
      break;

    case 3: // reward lickport licks that are more than 3 sec apart
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
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

   case 4: // auto reward 
      if (L_pin_state==LOW){
        L_pin_state = HIGH;
        digitalWrite(L_pin,HIGH);
        L_reward_timer = millis();
        r=1;
      }
      break;

   

   case 5: // cleaning/loading lickports
      digitalWrite(L_pin,HIGH);
      break;

   case 6: //  turn solenoids off 
      digitalWrite(L_pin,LOW);
      break;

   case 7: // turn LED on
      if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_state=HIGH;
        led_timer = millis();
      }
      break;

      
   case 9: // start collecting ttl0's for syncing
      //if (scan_flag>0) {
      //  scan_flag = 0;
      //} else {
        scan_flag = 1;
      //}
      
      break;

   case 10: // cmd 3 and 7 
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
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

    case 11: // air puff
       if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (puff_flag == 0)
      { // if reward not dispensed
        if (lc > 0)  { // if first lick is left
          puff_flag = 1;
          puff_state = HIGH;
          digitalWrite(puff_pin,HIGH);
          puff_timer = millis();
          r=3; // puff
        }
      }    
      break;

  case 12: // reward but don't click relay
    if (rflag == 0)
    { // if reward not dispensed
      if (lc > 0)  { // if first lick is left
        rflag = 1;
        L_pin_state = HIGH;
        digitalWrite(L_pin,HIGH);
        L_reward_timer = millis();
        r=1; // reward left
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
    Serial.print(r);              // print what the reward should was
    Serial.println("");
    lc = 0;
    r = 0;
  }
}








