const int f = 12; // high pin for forward motion
const int b = 11; // high pin for backward motion 
const int sol = 10;

void setup() {
  // make relay pins outputs
  pinMode(f,OUTPUT);
  pinMode(b,OUTPUT);
  pinMode(sol,OUTPUT);
  pinMode(sol,HIGH);
  // begin serial port
  Serial.begin(9600); 

}

void loop() {
  // make sure the actuator isn't moving
  pinMode(f, HIGH);
  pinMode(b, HIGH); 
  
  if (Serial.available()>0) { // If there is a pulse from Unity

    int cmd = Serial.read()-'0'; // convert command to integers

    switch (cmd) {

      case 0: // do nothing
        break;

      case 1: // move forward (make 'f' key)
        pinMode(f,HIGH);
        pinMode(b, LOW);
        delay(500);
        break;
        //pinMode(f, HIGH);
        //pinMode(b, HIGH);

      case 2: // move bacward  (make 'b' key)
        pinMode(b,HIGH);
        pinMode(f, LOW); 
        delay(500); 
        break;

        //pinMode(b, HIGH);
        //pinMode(f, HIGH);

      case 3: // pulse forward (make left arrow)
        pinMode(f, HIGH);
        pinMode(b, LOW);
        delay(50);
        break;
        //pinMode(b, HIGH);
        //pinMode(f, HIGH);

      case 4: // pulse backward (make right arrow)
        pinMode(b, HIGH);
        pinMode(f, LOW);
        delay(50);
        break;
        //pinMode(b, HIGH);
        //pinMode(f, HIGH);
    }
  }

}
