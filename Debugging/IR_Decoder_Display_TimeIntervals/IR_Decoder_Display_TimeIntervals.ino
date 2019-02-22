int irInput;
int lastIRInput;
int intervalCounter;
int totalIntervalCounter;
int timevalue = 1000;
int cutoff = 500;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(A1, INPUT);
  intervalCounter = 0;
  totalIntervalCounter = timevalue;
}

void loop() {
  lastIRInput = irInput;
  irInput = analogRead(A1);
  
  //Starts to collect data for only timevalue time
  if (irInput > cutoff && totalIntervalCounter >= timevalue){
    totalIntervalCounter = 0;
  }
  
  //arduino is now collecting data
  if (totalIntervalCounter < timevalue){
      //Serial.println(irInput); //use this to print one datapoint every clock oscelation
      totalIntervalCounter++;
      
      //Signal is still high or low, so update the counter(timer)
      if (irInput > cutoff && lastIRInput > cutoff || irInput <= cutoff && lastIRInput <= cutoff){
        intervalCounter++;
      } 
      
      //if the reading switches from low to high, display the counter for time low 
      else if (irInput > cutoff && lastIRInput <= cutoff){ 
            Serial.print("OFF time: ");
            Serial.println(intervalCounter);
            
            
            
            //since the signal just switched from low to high, the counter is reset to start counting the amount of time the signal is high
            intervalCounter = 0;
      }

      
      //if the reading switches from high to low, display the counter for time high
      else if(irInput <= cutoff && lastIRInput > cutoff){
          Serial.print("ON time: ");
          Serial.println(intervalCounter);
          
          intervalCounter = 0;
      }
  }
}

