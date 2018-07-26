#include <CapacitiveSensor.h>


CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 1 megohm resistor between pins 11 & 10, pin 10 is sensor pin
CapacitiveSensor   R_port = CapacitiveSensor(6, 5);       // 1 megohm resistor between pins 6 & 5, pin 5 is sensor pin

byte L_lick = 0; // lick variable
byte R_lick = 0; //

long th = 1000; // threshold
long lc = 0; // left lick count
long rc = 0; // right lick count

// rotary encoder stuff
int val; 
int encoder0PinA = 12;
int encoder0PinB = 13;
int encoder0Pos = 0;
int encoder0PinALast = LOW;
int n = LOW;
int incomingByte = 0;   


void setup() {
   pinMode (encoder0PinA,INPUT);
   pinMode (encoder0PinB,INPUT);
   Serial.begin (57600);

}

void loop() {

  // read capacitance
  long start = millis();
  long L_val =  L_port.capacitiveSensor(50);
  long R_val =  R_port.capacitiveSensor(30);

  if (L_val >= th ) { // if the value is greater than th count licks
    lc++;
  }
  if (R_val >= th) {
    rc++;

  }

  // read rotary encoder
  n = digitalRead(encoder0PinA);
  if ((encoder0PinALast == LOW) && (n == HIGH)) {
     if (digitalRead(encoder0PinB) == LOW) {
       encoder0Pos--;
     } else {
       encoder0Pos++;
     }
   }

  
  if (Serial.available()>0)
  {
    incomingByte = Serial.read();
   
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(rc);                  // print sensor output 2
    Serial.print("\t");
    Serial.print(encoder0Pos);
    Serial.println("");
    lc = 0;
    rc = 0;
    encoder0Pos = 0;
  }
   
   //Serial.println(encoder0Pos);
   encoder0PinALast = n;


}




