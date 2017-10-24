
int incomingByte = 0; 

bool mem0 = false;
bool mem1 = false;
bool mem3 = false;
bool mem9 = false;
bool mem10 = false;
bool mem11 = false;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(11,OUTPUT);
  pinMode(10,OUTPUT);
  pinMode(9,OUTPUT);
  pinMode(3,OUTPUT);
  pinMode(0,OUTPUT);
  pinMode(1,OUTPUT);
}

bool switchPin(int pinNum, bool mem) {
  if (mem) {
    if (pinNum == 3) {
      analogWrite(3,0);
    } else {
      digitalWrite(pinNum,LOW);
    }
    return false;
  } else {
    if (pinNum == 3) {
      analogWrite(3,20);
    } else {
      digitalWrite(pinNum,HIGH);
    }
    return true;
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available()>0) { // if new Unity frame
  int pin = Serial.read()-'0';

  switch(pin) {

      case 0:
        mem0 = switchPin(0, mem0);
        break;

      case 1:
        mem1 = switchPin(1, mem1);
        break;

      case 3:
        mem3 = switchPin(3, mem3);
        break;
        
      case 9:
        mem9 = switchPin(9, mem9);
        break;

      case 10:
        mem10 = switchPin(10, mem10);
        break;

      case 11:
        mem11 = switchPin(11, mem11);
        break;
        
    
    } 
  }
}


