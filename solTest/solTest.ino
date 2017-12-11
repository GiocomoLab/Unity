

//#include <Servo.h>
//Servo lickServo;
const int sol = 13;
//int pos=0;
// default position is forward
void setup() {
  // make relay pins outputs
  pinMode(sol, OUTPUT);
  // begin serial port
  //Serial.begin(9600);
  //lickServo.attach(9, 590, 2350);
  //lickServo.write(pos);

}

void loop() {

        delay(2000);
        pinMode(sol, LOW);
        delay(2000);
        pinMode(sol, HIGH);
        
}
