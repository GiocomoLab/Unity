/* Read Quadrature Encoder
 *  Connect Encoder PinA to 12, PinB to 13, and the brown wire to +5V
 *  
 *  modified from: Sketch by max wolf / www.meso.net
  * v. 0.1 - very basic functions - mw 20061220
 */

// set pins to read encoder
int encoder_pinA = 12;
int encoder_pinB = 13;
// initialize position and pin values
int pos = 0;
int pinA_last = LOW;
int pinA_curr = LOW;
int incomingByte = 0;

void setup() {
  pinMode(encoder_pinA,INPUT); // set encoder pins as inputs
  pinMode(encoder_pinB, INPUT);
  Serial.begin(57600); // seed serial port
}

void loop() {
  pinA_curr = digitalRead(encoder_pinA);
  if((pinA_last==LOW) && (pinA_curr == HIGH)) { // if pin A transitions from low to high
    if (digitalRead(encoder_pinB) == LOW) { // if pin B trails pin A (clockwise)
      pos=-1; // move backward
    } else { // if pin B lead pin A (counter-clockwise)
      pos=1; // move forward
    }
  }
  if (Serial.available()>0){ // only print if there is information available from the serial port
      incomingByte = Serial.read(); // read incoming bytes
      Serial.println(pos); // print position
      
  }
  pinA_last = pinA_curr; // update last pinA value
}
