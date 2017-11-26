
const int sol = 10;
// default position is forward
void setup() {
  // make relay pins outputs
  pinMode(sol,OUTPUT);
  // begin serial port
  Serial.begin(9600); 

}

void loop() {
  
  if (Serial.available()>0) { // If there is a pulse from Unity

    int cmd = Serial.read()-'0'; // convert command to integers

    switch (cmd) {

      case 0: // do nothing
        break;

      case 1: // move forward (make 'f' key)
        pinMode(sol,LOW);
        break;
        
      case 2: // move bacward  (make 'b' key)
        pinMode(sol, HIGH); 
        break;

        
    }
  }

}
