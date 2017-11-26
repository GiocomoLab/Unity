#include <CapacitiveSensor.h>

CapacitiveSensor   L_port = CapacitiveSensor(11, 10);       // 2x1 megohm resistor between 
                                                          // pins 11 & 10, pin 10 is sensor pin,
                                                          //100 pF capacitor was placed in parallel to increase signal to noise
CapacitiveSensor   R_port = CapacitiveSensor(6, 5);       // 2x1 megohm resistor between pins 6 & 5, pin 5 is sensor pin

// single lickport for initial stages of training
CapacitiveSensor S_port = CapacitiveSensor(9, 3);   

int trial_begin = 0; // flag for start of trial
int rflag = 0; // flag to avoid multiple rewards
int r = 0; // tell unity whether to deliver reward (1 = left, 2 = right)

long th = 2500; // threshold
long lc = 0; // left lick count
long rc = 0; // right lick count
long sc = 0; // single lick count;

long llc = 0; // cumulative left lick count during trial
long rrc = 0; // cumulative right lick count during trial
long ssc = 0; // cumulative single lick count 
 
long timer = 0; // to make sure mouse doesn't get rewarded for licking randomly during running
long start = 0;

int lflag = 0;
int lstart = 0;
int rstart = 0;
int sstart = 0; 

int incomingByte = 0;  // cmd coming in from Unity

void setup() {
  Serial.begin(9600);
}

void loop() 
{

  // double lickport
  long L_val =  L_port.capacitiveSensor(50);
  long R_val =  R_port.capacitiveSensor(50);
  // single lickport
  long S_val = S_port.capacitiveSensor(50);
  
  if (L_val >= th) { // count licks since last Unity frame
    lc++;
  }
  if (R_val >= th) { 
    rc++;
  }
  if (S_val >= th) {
    sc++;
  }
  

  if (Serial.available()>0) { // if new Unity frame
    int cmd = Serial.read()-'0';
   
    switch (cmd) {

      case 0: // just count licks 
        break;

      case 1: // left reward 
        if (trial_begin == 0) { // if first frame  of reward trial
          trial_begin++;
          start = millis();
        } 
        timer = millis()-start;
        if ((timer >= 5) & (llc + rrc == 0) & (rflag == 0))
        { // if past 50 milliseconds, no licks yet, and reward not dispensed
          if (lc > 0 & rc < 1)  { // if first lick is left
            rflag = 1;
            r =  1; // reward left
          }
          if (rc > 0 & lc < 1) 
          { 
            rflag = 1; 
            r=4; } // if first lick is right prevent reward - r=4 is incorrect right
        }
        
        llc += lc;
        rrc += rc;    
        break;
      
      case 2: // right reward
       if (trial_begin == 0) { // if first frame  of reward trial
          trial_begin++;
          start = millis();
        } 
        timer = millis()-start;
        
        if ((timer >= 5) & (llc + rrc == 0) & (rflag == 0))
        { // if past 50 milliseconds, no licks yet, and reward not dispensed
          if (rc > 0 & lc < 1) { // if first lick is right
            rflag = 1;
            r = 2; // reward right
          }
          if (lc > 0 & rc < 1) 
          { 
            rflag = 2;
            r = 3;} // if  first lick is left prevent reward - r=3 is incorrect left
        }
        
        llc += lc;
        rrc += rc;    
        break; 

      case 3: // reset flags
        llc = 0;
        rrc = 0;
        ssc = 0;
        timer = 0;
        trial_begin = 0;
        rflag = 0;
        break;

      case 4: // reward double lickport licks that are more than 3 sec apart

        if (lc>0 & rc < 1 & rflag == 0) { // if lick left
          rflag  = 1;
          r = 1;
          start = millis();
        } else if ( rc > 0 & lc<1 & rflag == 0) { // if lick right
          rflag = 1;
          r = 2;
          start = millis();
        } else {
          r = 0;
        }

        if (millis()-start > 3000) { // reset flag if its been more than 1 sec
          rflag = 0;
        }
        
        break;

      case 5: // reward a lick left even if its not first
        if (trial_begin == 0) { // if first frame  of reward trial
          trial_begin++;
          start = millis();
        } 
        
        timer = millis()-start;
        if ((timer >= 5) & (rflag == 0))
        { // if past 50 milliseconds, no licks yet, and reward not dispensed
          if (lc > 0 & rc < 1)  { // if first lick is left
            rflag = 1;
            r =  1; // reward left
          }
          
        }
            
        break;

      case 6: // reward a lick right even if its not first
        if (trial_begin == 0) { // if first frame  of reward trial
          trial_begin++;
          start = millis();
        } 
        
        timer = millis()-start;
        if ((timer >= 5) & (rflag == 0))
        { // if past 50 milliseconds, no licks yet, and reward not dispensed
          if (rc > 0 & lc < 1)  { // if first lick is left
            rflag = 1;
            r =  1; // reward left
          }
         
        }
        break;

      case 7: // reward first lick regardless of side
        if (trial_begin == 0) { // if first frame  of reward trial
          trial_begin++;
          start = millis();
        } 
        timer = millis()-start;
        
        if ((timer >= 5) & (llc + rrc == 0) & (rflag == 0))
        { // if past 50 milliseconds, no licks yet, and reward not dispensed
          if (rc > 0 & lc < 1) { // if first lick is right
            rflag = 1;
            r = 2; // reward right
          }
          if (lc > 0 & rc < 1) // if first lick is left
          { 
            rflag = 2;
            r = 1;} // reward left
        }
        
        llc += lc;
        rrc += rc;    
        break;
    

      case 8: // reward licks to the single port that are more than 5 sec apart 
        if (sc>0 & rflag == 0) { // if lick left
            rflag  = 1;
            r = 1;
            start = millis();
          } else {
            r = 0;
          }
  
        if (millis()-start > 3000) { // reset flag if its been more than 1 sec
            rflag = 0;
        }
        ssc += sc;  
        break;

      case 9: // reward first lick to single port 
         if (trial_begin == 0) { // if first frame  of reward trial
            trial_begin++;
            start = millis();
          } 
          timer = millis()-start;
          if ((timer >= 5) & (ssc == 0) & (rflag == 0))
          { // if past 50 milliseconds, no licks yet, and reward not dispensed
            if (sc > 0)  { // if first lick 
              rflag = 1;
              r =  1; // reward left
            }
          }
        
         ssc += sc;
         break;

      
      
      
    }


    
    Serial.print(lc);                  // print sensor output 1
    Serial.print("\t");
    Serial.print(rc);                  // print sensor output 2
    Serial.print("\t");
    Serial.print(sc);                 // print sensor output 3
    Serial.print("\t");
    Serial.print(r);              // print whether reward should be
    Serial.println("");
    delay(10);
    lc = 0;
    rc = 0;
    sc = 0;
    r = 0;

    
  }
}
