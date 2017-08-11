
#include <CapacitiveSensor.h>

/*

*/


CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 1 megohm resistor between pins 11 & 10, pin 10 is sensor pin
CapacitiveSensor   R_port = CapacitiveSensor(6, 5);       // 1 megohm resistor between pins 6 & 5, pin 5 is sensor pin

//byte L_lick = 0; // lick variable
//byte R_lick = 0; //

long th = 1000; // threshold
long lc = 0; // left lick count
long rc = 0; // right lick count

int incomingByte = 0; 
//int L_out = 3;
//int R_out = 9;

void setup()
{
//  pinMode(L_out,OUTPUT);
  //pinMode(R_out,OUTPUT);
  

  Serial.begin(57600);

}

void loop()
{
  long start = millis();
  long L_val =  L_port.capacitiveSensor(50);
  long R_val =  R_port.capacitiveSensor(50);


  //analogWrite(L_out,L_val/10.0);
  //analogWrite(R_out,R_val/10.0);
  if (L_val >= th ) {
    lc++;
  }
  if (R_val >= th) {
    rc++;

  }
  //Serial.print(millis() - start);        // check on performance in milliseconds
  //Serial.print("\t");                    // tab character for debug window spacing
  if (Serial.available()>0)
  {
    incomingByte = Serial.read();
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(rc);                  // print sensor output 2
    Serial.println("");
    delay(10);
    lc = 0;
    rc = 0;
  }
  //        // arbitrary delay to limit data to serial port
  //}
}

