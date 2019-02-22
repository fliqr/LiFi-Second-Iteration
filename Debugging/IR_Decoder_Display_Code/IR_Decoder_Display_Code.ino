//A CODE FROM AN IR REMOTE WILL START WITH A HIGH SIGNAL FOR ABOUT 0.22569ms (26 TIME UNITS AT 115200BAUD)
//ALL OFF TIMES AFTER WILL BE 0.0472ms (4 OR 5 UNITS)
//ALL ON TIMES WILL BE EITHER 0.1693ms (19 OR 20 UNITS) FOR A 1 AND 0.026ms (3 OR 4 UNITS) FOR A 0
//WHEN HOLDING DOWN ON A BUTTON THERE WILL BE A 2.899ms (334 UNIT) OFF TIME

int irInput;
int lastIRInput;
int totalIntervalCounter;
int intervalCounter;
int timevalue = 1000;
int code2[30][10];
int code[30];
int codeIndex;
int codeNumber;
int codeSize;
int cutoffValue = 500;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(A1, INPUT);
  totalIntervalCounter = timevalue;
  intervalCounter = 0;
  codeSize = (sizeof(code)/sizeof(code[0]));
  codeIndex = codeSize;
  codeNumber = 0;
}

void loop() {
  lastIRInput = irInput;
  irInput = analogRead(A1);
  
  //Starts to collect data for only timevalue time
  if (irInput > cutoffValue && totalIntervalCounter >= timevalue){
    totalIntervalCounter = 0;
  }
  
  //arduino is now collecting data
  if (totalIntervalCounter < timevalue){
      //Serial.println(irInput); //use this to print one datapoint every clock oscelation
      totalIntervalCounter++;
      
      //Signal is still high or low, so update the counter(timer)
      if (irInput > cutoffValue && lastIRInput > cutoffValue || irInput <= cutoffValue && lastIRInput <= cutoffValue){
        intervalCounter++;
      } 
      
      //if the reading switches from low to high, display the counter for time low 
      else if (irInput > cutoffValue && lastIRInput <= cutoffValue){ 
            //Serial.print("OFF time: ");
            //Serial.print(intervalCounter);
            //Serial.print(" ");
            //Serial.print(codeIndex);
            //Serial.print(" ");
            //Serial.println(codeNumber);
            
            //if the break in signal time is over 30 units of time, and the index is at the highest value, 
            //then the code has not been analyzing the code and will start analyzing the code
            if(intervalCounter > 30 && codeIndex == codeSize){
                codeIndex = 0;
            } else if (codeIndex >= 0 && codeIndex < codeSize){  //If the code is being collected...
                if (intervalCounter > 30){//the gap in code is too long and data will stop being collected
                  codeIndex = codeSize - 1;//-1 because codeIndex will increment tipping it over the scope.
                  
                  //Print all collected data
                 printCode();
                  codeNumber++;
                } else if (intervalCounter > 10){//The remote is sending a 1
                  //code[codeIndex][codeNumber] = 1;
                  code[codeIndex] = 1;
                } else {                         //The remote is sending a 0
                  //code[codeIndex][codeNumber] = 0;
                  code[codeIndex] = 0;
                }
                codeIndex++;
            } 
            
            //since the signal just switched from low to high, the counter is reset to start counting the amount of time the signal is high
            intervalCounter = 0;
      }

      
      //if the reading switches from high to low, display the counter for time high
      else if(irInput <= cutoffValue && lastIRInput > cutoffValue){
          //Serial.print("ON time: ");
          //Serial.println(intervalCounter);

          if(codeIndex >= 0 && codeIndex < codeSize && intervalCounter > 30){ 
            codeIndex = codeSize;
            printCode();
          }
          
          intervalCounter = 0;
      }
  }
}

void printCode(){
   for (int i = 0; i < codeSize; i++){
      if (code[i] == 1){
          Serial.print("1");
      } else {
          Serial.print("0");
      }
  }
  Serial.println();
                  
}

