/* Read Quadrature Encoder
  * Connect Encoder to Pins encoder0PinA, encoder0PinB, and +5V.
  *
  */  


 int val; 
 int encoder0PinA = 10;
 int encoder0PinB = 11;
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
   n = digitalRead(encoder0PinA);
   if ((encoder0PinALast == LOW) && (n == HIGH)) { // if pinA switches 
     if (digitalRead(encoder0PinB) == LOW) { // if pinA leads pinB; going backwards
       encoder0Pos--;
     } else { // else; going forwards
       encoder0Pos++;
     }
   }
   
   if (Serial.available()>0) // if Unity frame occured
   {
     //read the incoming byte:
     incomingByte = Serial.read(); // clear serial line
     Serial.println(encoder0Pos); // send new delta position
     encoder0Pos = 0; // reset
   }
   
   encoder0PinALast = n; // reset pin value
 }
 
