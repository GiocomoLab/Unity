const int f = 11; 
const int b = 12;

void setup() {
  // put your setup code here, to run once:
  pinMode(f,OUTPUT);
  pinMode(b,OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  digitalWrite(f,LOW);
  digitalWrite(b,HIGH);
  delay(500);

  digitalWrite(f,HIGH);
  digitalWrite(b,HIGH);
  delay(2000);

  digitalWrite(f,HIGH);
  digitalWrite(b,LOW);
  delay(500);

  digitalWrite(f,HIGH);
  digitalWrite(f,HIGH);
  delay(2000);
}
