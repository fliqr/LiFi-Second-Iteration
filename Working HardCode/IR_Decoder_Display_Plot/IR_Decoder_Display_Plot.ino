
int irInput;
int inputPin = 1;

//ExponentialFilter<long> ADCFilter(2, 0);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(A0, INPUT);
}

void loop() {
  irInput = analogRead(A0);          
  Serial.println(irInput);

}

