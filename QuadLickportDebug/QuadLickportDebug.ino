#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(10, 11); 



int r = 0; // tell unity which case was executed
long th = 500; // threshold
long lc = 0; // lick count
int rflag = 0; // reward flag

long start = 0; // timer for case 3

const int slider_pin = 4; // slide table
const int milk_pin = 8;
const int quin_pin = 7;
const int vac_pin = 2;
const int water_pin = 12;
const int led_pin =3;


const int slider_len = 2000; // length of reward availability in milliseconds
const int led_len = 2500; // length of LED on
const int water_len = 1000; // length of airpuff
const int vac_len = 3000;
const int reward_len = 50;

int slider_pin_state = LOW;
int led_pin_state = LOW;
int water_pin_state = LOW;
int vac_pin_state = LOW;
int milk_pin_state = LOW;
int quin_pin_state= LOW;

long slider_timer = 0;
long led_timer = 0;
long water_timer = 0;
long vac_timer = 0;
long milk_timer = 0;
long quin_timer = 0;

int cmd = 0;
int slider_flag = 0;
int led_flag = 0;
int water_flag = 0;
int vac_flag = 0;
int milk_flag = 0;
int quin_flag = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(57600);
  L_port.set_CS_AutocaL_Millis(20);


  pinMode(slider_pin,OUTPUT);
  digitalWrite(slider_pin,LOW);

  pinMode(milk_pin,OUTPUT);
  digitalWrite(milk_pin,LOW);

  pinMode(quin_pin,OUTPUT);
  digitalWrite(quin_pin,LOW);

  pinMode(vac_pin,OUTPUT);
  digitalWrite(vac_pin,LOW);

  pinMode(water_pin,OUTPUT);
  digitalWrite(water_pin,LOW);

  pinMode(led_pin,OUTPUT);
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

  if (milk_pin_state == HIGH) {
    if (millis() - milk_timer > reward_len) {
      digitalWrite(milk_pin,LOW);
      milk_pin_state = LOW;
    }
  }

  if (quin_pin_state == HIGH) {
    if (millis() - quin_timer> reward_len) {
      digitalWrite(quin_pin,LOW);
      quin_pin_state = LOW;
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

 switch (cmd) {

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
          milk_pin_state = HIGH;
          digitalWrite(milk_pin,HIGH);
          milk_timer = millis();
          r=1; // milk reward
        }
      }    
      break;
    
    
    case 2: // reset flags
      rflag = 0;
      slider_flag = 0;
      milk_flag = 0;
      quin_flag = 0;
      
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
        milk_pin_state = HIGH;
        digitalWrite(milk_pin,HIGH);
        milk_timer = millis();
        start = millis();
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

   case 4: // auto reward 
      milk_pin_state = HIGH;
      digitalWrite(milk_pin,HIGH);
      milk_timer = millis();
      break;

   

   case 5: // cleaning/loading lickports
      digitalWrite(milk_pin,HIGH);
      digitalWrite(water_pin,HIGH);
      digitalWrite(quin_pin,HIGH);
      break;

   case 6: //  turn solenoids off 
      digitalWrite(milk_pin,LOW);
      digitalWrite(water_pin,LOW);
      digitalWrite(quin_pin,LOW);
      break;

   case 7: // turn LED on
      if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_pin_state=HIGH;
        led_timer = millis();
      }
      break;

   case 10: // cmd 3 and 7 
    if (led_flag == 0) {
        analogWrite(led_pin,20);
        led_pin_state=HIGH;
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
        milk_pin_state = HIGH;
        digitalWrite(milk_pin,HIGH);
        milk_timer = millis();
        start = millis();
      } 

      if (millis()-start > 3000) { // reset flag if its been more than 3 sec
        rflag = 0;
      }
      
      break;

    case 11: // quinine
       if (slider_pin_state==LOW & slider_flag==0) { // slider is back
        digitalWrite(slider_pin,HIGH); //  move it forward
        slider_pin_state=HIGH;
        slider_timer=millis();
        slider_flag =1;
      }
      
      if (quin_flag == 0)
      { // if reward not dispensed
        if (lc > 0)  { // if first lick is left
          quin_flag = 1;
          quin_pin_state = HIGH;
          digitalWrite(quin_pin,HIGH);
          quin_timer = millis();
          r=3; // quinine
        }
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
  }

  if (Serial.available()>0) { // if new Unity frame
   
    
    cmd = Serial.parseInt();    

    
    
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(r);              // print what the reward should was
    Serial.println("");
    lc = 0;
    r = 0;
  }
}
  

 

