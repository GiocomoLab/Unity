const int s = 10; 

void setup() {
  // put your setup code here, to run once:
  pinMode(s,OUTPUT);
  
}

void loop() {
  // put your main code here, to run repeatedly:
  digitalWrite(s,LOW);
  delay(500);

  digitalWrite(s,HIGH);
  delay(2000);

 
}
