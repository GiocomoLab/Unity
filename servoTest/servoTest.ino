#include <Servo.h>

Servo testservo;

uint32_t next;
int currpos=0;

void setup() {
  // put your setup code here, to run once:
  testservo.attach(9,600,2300);
  pinMode(10,OUTPUT);
  next = millis() +500;
}

void loop() {
  pinMode(10,HIGH);
  // put your main code here, to run repeatedly:
  static bool rising = true;
  //testservo.write(currpos);

  if (millis() > next)
  {
    if (rising) {
      testservo.write(180);
      rising = false;  
      currpos = 0;
      
    }
    else 
    {
      testservo.write(180);
      rising = true;
      currpos = 180;
    }

    next += 3000;
  
  }

}
