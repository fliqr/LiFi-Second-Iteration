#include <Filter.h>

int irInput;
int intervalCounter;
int totalIntervalCounter;
int timevalue = 400;
int inputArray[100];
int filtered;

ExponentialFilter<long> ADCFilter(2, 0);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(A0, INPUT);
  intervalCounter = 0;
  totalIntervalCounter = timevalue;
}

void loop() {
  irInput = analogRead(A0);
  ADCFilter.Filter(irInput);
  filtered = ADCFilter.Current();
  
  for(int i = 0; i < 100; i++){
    inputArray[i] = analogRead(A0);
  }
  for(int i = 0; i < 100; i++){
    Serial.println(inputArray[i]);
  }
}

