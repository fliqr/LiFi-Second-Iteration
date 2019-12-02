
int irInput;
int inputPin = 1;
int ind = 0;
const int len = 100;
int temp[len];

//ExponentialFilter<long> ADCFilter(2, 0);


void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(A0, INPUT);
}

void loop() {    
    Serial.println(analogRead(A0));
}

