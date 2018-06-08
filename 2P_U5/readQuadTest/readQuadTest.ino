/* Read Quadrature Encoder
  * Connect Encoder to Pins encoder0PinA, encoder0PinB, and +5V.
  *
  * Sketch by max wolf / www.meso.net
  * v. 0.1 - very basic functions - mw 20061220
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
   //Serial.begin(115200);
 } 

 void loop() { 
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
     // read the incoming byte:
     incomingByte = Serial.read();
     Serial.println(encoder0Pos);
     encoder0Pos = 0;
  
   }
   
   
   encoder0PinALast = n;
  
 }
