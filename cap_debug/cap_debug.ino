#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 1 megohm resistor between pins 11 & 10, pin 10 is sensor pin
CapacitiveSensor   R_port = CapacitiveSensor(6, 5);       // 1 megohm resistor between pins 6 & 5, pin 5 is sensor pin
//CapacitiveSensor S_port = CapacitiveSensor(11, 10);   

int trial_begin = 0; // flag for start of trial
int rflag = 0; // flag to avoid multiple rewards
int r = 0; // tell unity whether to deliver reward (1 = left, 2 = right)

long th = 100; // threshold
long lc = 0; // left lick count
//long rc = 0; // right lick count

long llc = 0; // cumulative left lick count during trial
//long rrc = 0; // cumulative right lick count during trial
 
long timer = 0; // to make sure mouse doesn't get rewarded for licking randomly during running
long start = 0;

int lflag = 0;
int lstart = 0;
//int rstart = 0;

int incomingByte = 0;  // cmd coming in from Unity

void setup() {
  Serial.begin(115200);
  //pinMode(7,OUTPUT);
  //pinMode(8,OUTPUT);
  pinMode(9,OUTPUT);
//  S_port.set_CS_Timeout_Millis(20);
 // R_port.set_CS_Timeout_Millis(15);
  L_port.set_CS_AutocaL_Millis(20);
  R_port.set_CS_AutocaL_Millis(20);
}

void loop() 
{
  //pinMode(7,HIGH);
  //pinMode(8,HIGH);
  analogWrite(9,20);
  long L_val =  L_port.capacitiveSensor(30);
  long R_val =  R_port.capacitiveSensor(30);
  //long S_val = S_port.capacitiveSensor(50);
  
    Serial.print(L_val);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(R_val);                  // print sensor output 2
    //Serial.print("\t");
   //Serial.print(S_val);                 // print sensor output 3
   // Serial.print("\t");
    
    Serial.println("");
  //if (R_val>5000) {
  // R_port.reset_CS_AutoCal();
  //}
  //if (L_val>5000) {
 //  L_port.reset_CS_AutoCal(); 
 // }
   
   
   // delay(10);
    lc = 0;
  // rc = 0;
  //  r = 0;

    
  
}
