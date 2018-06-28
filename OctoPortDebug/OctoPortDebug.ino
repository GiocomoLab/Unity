#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(10, 11);
CapacitiveSensor   R_port = CapacitiveSensor(5,6);



int r = 0; // tell unity which case was executed
long th = 5; // threshold
long lc = 0; // lick count
long rc = 0; 
int rflag = 0; // reward right flag
int lflag = 0 ; // reward left flag

long start = 0; // timer for case 3

const int slider_pin = 4; // slide table
const int L_milk_pin = 13;
const int L_quin_pin = 7;
const int R_milk_pin = 8;
const int R_quin_pin = 12;
const int vac_pin = 2;
const int water_pin = 9;
const int led_pin =3;


const int slider_len = 2000; // length of reward availability in milliseconds
const int led_len = 2500; // length of LED on
const int water_len = 500; // length of airpuff
const int vac_len = 4000;
const int reward_len = 25;

int slider_pin_state = LOW;
int led_pin_state = LOW;
int water_pin_state = LOW;
int vac_pin_state = LOW;
int L_milk_pin_state = LOW;
int R_milk_pin_state = LOW;
int L_quin_pin_state= LOW;
int R_quin_pin_state = LOW;


long slider_timer = 0;
long led_timer = 0;
long water_timer = 0;
long vac_timer = 0;
long L_milk_timer = 0;
long R_milk_timer = 0;
long L_quin_timer = 0;
long R_quin_timer = 0;

int cmd = 0;
int slider_flag = 0;
int led_flag = 0;
int water_flag = 0;
int vac_flag = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(57600);
  L_port.set_CS_AutocaL_Millis(30);
  R_port.set_CS_AutocaL_Millis(30);

  pinMode(slider_pin,OUTPUT);
  pinMode(L_milk_pin,OUTPUT);
  pinMode(L_quin_pin,OUTPUT);
  pinMode(R_milk_pin,OUTPUT);
  pinMode(R_quin_pin,OUTPUT);
  pinMode(vac_pin,OUTPUT);
  pinMode(water_pin,OUTPUT);
  pinMode(led_pin,OUTPUT);
  
  digitalWrite(slider_pin,LOW);
  digitalWrite(L_milk_pin,LOW);
  digitalWrite(L_quin_pin,LOW);
  digitalWrite(R_milk_pin,LOW);
  digitalWrite(R_quin_pin,LOW);
  digitalWrite(vac_pin,LOW);
  digitalWrite(water_pin,LOW);
  analogWrite(led_pin,0);
  
  
}

void loop() {
  // check pin states

  if ((slider_pin_state == HIGH)){
     if (millis()-slider_timer>slider_len) {
      digitalWrite(slider_pin, LOW);
      slider_pin_state = LOW;
     }   
  }

  if (led_pin_state == HIGH) {
    if (millis() - led_timer>led_len){
      analogWrite(led_pin,0);
      led_pin_state=LOW;
      led_flag = 0;
    }
  }

  if (L_milk_pin_state == HIGH) {
    if (millis() - L_milk_timer > reward_len) {
      digitalWrite(L_milk_pin,LOW);
      L_milk_pin_state = LOW;
    }
  }

  if (R_milk_pin_state == HIGH) {
    if (millis() - R_milk_timer > reward_len) {
      digitalWrite(R_milk_pin,LOW);
      R_milk_pin_state = LOW;
    }
  }

  if (L_quin_pin_state == HIGH) {
    if (millis() - L_quin_timer> reward_len) {
      digitalWrite(L_quin_pin,LOW);
      L_quin_pin_state = LOW;
    }
  }

  if (R_quin_pin_state == HIGH) {
    if (millis() - R_quin_timer> reward_len) {
      digitalWrite(R_quin_pin,LOW);
      R_quin_pin_state = LOW;
    }
  }

  if (water_pin_state == HIGH) {
    if (millis() - water_timer > water_len) {
      digitalWrite(water_pin,LOW);
      water_pin_state = 0;
      water_flag = 0;
    }
  }

  if (vac_pin_state == HIGH) {
    if (millis() - vac_timer > vac_len) {
      digitalWrite(vac_pin,LOW);
      vac_pin_state = LOW;
      vac_flag = 0;
    }
  }
  
  long L_val =  L_port.capacitiveSensor(30);
  if ((L_val >= th) | (L_val==-2)) { // count licks since last Unity frame
    lc++;
    
  }

  long R_val =  R_port.capacitiveSensor(30);
  if ((R_val >= th) | (R_val==-2)) { // count licks since last Unity frame
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
      
      if (lflag == 0  & rflag==0)
      { // if reward not dispensed
        if (lc > 0)  { // if first lick is left
          lflag = 1;
          L_milk_pin_state = HIGH;
          digitalWrite(L_milk_pin,HIGH);
          L_milk_timer = millis();
          r=1; // milk reward 
        }
        if (rc>0 & lc<1) { // first lick is right
          rflag =1;
          r=4;
          R_quin_pin_state = HIGH;
          digitalWrite(R_quin_pin,HIGH);
          R_quin_timer = millis();
        }
        
      }    
      break;

   case 2: // right reward 
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (rflag == 0 & lflag == 0)
      { // if reward not dispensed
        if (rc > 0)  { // if first lick is left
          rflag = 1;
          R_milk_pin_state = HIGH;
          digitalWrite(R_milk_pin,HIGH);
          R_milk_timer = millis();
          r=2; // milk reward right
        }
        if (lc>0 & rc<1) { // first lick is right
          lflag =1;
          r=3; // quinine left
          L_quin_pin_state = HIGH;
          digitalWrite(L_quin_pin,HIGH);
          L_quin_timer = millis();
        }
        
      }    
      break;
    
    
    case 3: // reset flags
      rflag = 0;
      lflag = 0;
      slider_flag = 0;
     
      
      break;

    case 4: // reward lickport licks that are more than 3 sec apart
      if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag = 1;
      }
      
      if (lc>0 & lflag == 0) { // if lick left
        lflag  = 1; rflag = 1;
        r = 1;
        L_milk_pin_state = HIGH;
        digitalWrite(L_milk_pin,HIGH);
        L_milk_timer = millis();
        start = millis();
      } 
      if (rc>0 & rflag ==0) { // if lick right
        lflag = 1; rflag=1;
        r = 2;
        R_milk_pin_state = HIGH;
        digitalWrite(R_milk_pin, HIGH);
        R_milk_timer = millis();
        start= millis();
      }

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0; lflag=0;
      }
      
      break;

   case 5: // milk left quinine right - both available
     if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (lflag == 0)
      { // if reward not dispensed
        if (lc > 0)  { // if any lick is left
          lflag = 1;
          L_milk_pin_state = HIGH;
          digitalWrite(L_milk_pin,HIGH);
          L_milk_timer = millis();
          r=1; // milk reward 
        }
      }
      if (rflag ==0) {
        if (rc>0 ) { // any lick is right
          rflag =1;
          r=4;
          R_quin_pin_state = HIGH;
          digitalWrite(R_quin_pin,HIGH);
          R_quin_timer = millis();
        }
        
      }    
      break;


   case 6: // milk right, quinine LEFT - both available 
     if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (lflag == 0)
      { // if reward not dispensed
        if (lc > 0)  { // if any lick is left
          lflag = 3;
          L_quin_pin_state = HIGH;
          digitalWrite(L_quin_pin,HIGH);
          L_quin_timer = millis();
          r=3; // milk reward 
        }
      }
      if (rflag ==0) {
        if (rc>0 ) { // any lick is right
          rflag =1;
          r=2;
          R_milk_pin_state = HIGH;
          digitalWrite(R_milk_pin,HIGH);
          R_milk_timer = millis();
        }
        
      }    
      break;

      
   case 7: // auto milk left 
      L_milk_pin_state = HIGH;
      digitalWrite(L_milk_pin,HIGH);
      L_milk_timer = millis();
      break;

   case 8: // auto milk right
      R_milk_pin_state = HIGH;
      digitalWrite(R_milk_pin,HIGH);
      R_milk_timer = millis();
      break;

   case 9: // cleaning/loading lickports
      digitalWrite(L_milk_pin,HIGH);
      digitalWrite(R_milk_pin,HIGH);
      digitalWrite(water_pin,HIGH);
      digitalWrite(L_quin_pin,HIGH);
      digitalWrite(R_quin_pin,HIGH);
      digitalWrite(vac_pin,HIGH);
      break;

   case 10: //  turn solenoids off 
      digitalWrite(L_milk_pin,LOW);
      digitalWrite(R_milk_pin,LOW);
      digitalWrite(water_pin,LOW);
      digitalWrite(L_quin_pin,LOW);
      digitalWrite(R_quin_pin,LOW);
      digitalWrite(vac_pin,LOW);
      break;

   case 11: // turn LED on
      if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_pin_state=HIGH;
        led_timer = millis();
      }
      break;


    case 12: // rinse

      if (water_pin_state == LOW & water_flag == 0 & slider_pin_state == LOW) { //rinse hasn't started
        digitalWrite(water_pin,HIGH); // start rinse
        water_pin_state = HIGH;
        water_timer = millis();
        water_flag = 1;
      }

      if (vac_pin_state == LOW & vac_flag == 0 & slider_pin_state == LOW) { // vacuum hasn't started
        digitalWrite(vac_pin,HIGH);
        vac_pin_state=HIGH;
        vac_timer = millis();
        vac_flag = 1;
      }
      break;
  }

  if (Serial.available()>0) { // if new Unity frame
   
    
    cmd = Serial.parseInt();    

    
    
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(rc);
    Serial.print("\t");
    Serial.print(r);              // print what the reward should was
    Serial.println("");
    lc = 0;
    rc =0;
    r = 0;
  }
}
  

 

