

#include <Servo.h>
Servo lickServo;
const int sol = 10;
int pos=0;
// default position is forward
void setup() {
  // make relay pins outputs
  pinMode(sol, OUTPUT);
  // begin serial port
  Serial.begin(9600);
  lickServo.attach(9, 600, 2300);
  lickServo.write(pos);

}

void loop() {

  if (Serial.available() > 0) { // If there is a pulse from Unity

    int cmd = Serial.read() - '0'; // convert command to integers

    switch (cmd) {

      case 0: // do nothing
        break;

      case 1: // move forward (make 'f' key)
        pinMode(sol, LOW);
        break;

      case 2: // move bacward  (make 'b' key)
        pinMode(sol, HIGH);
        break;

      case 3: // 0 degree servo
        lickServo.write(0);
        
        break;

      case 4: // 180 degree servo
        lickServo.write(180);
        
        break;




    }
  }

}


