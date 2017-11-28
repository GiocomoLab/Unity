#include <Servo.h>

Servo testservo;

uint32_t next;
int currpos = 0;
bool firstLoop = true;
int pos;
int pause = 50;

void setup() {
  // put your setup code here, to run once:
  testservo.attach(9, 600, 2400);
  pinMode(10, OUTPUT);
  next = millis() + 2000;
  //testservo.writeMicroseconds(600);
  Serial.begin(9600);
}

void loop() {
  pinMode(10, HIGH);
  // put your main code here, to run repeatedly:
  //static bool rising = true;
  delay(2000);
  testservo.write(0);
  delay(2000);
  testservo.write(180);

  }




