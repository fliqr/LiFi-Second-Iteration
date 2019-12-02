const int pin = 13;
const int baud = 9600;
String incomingByte = "";

void setup() {
  pinMode(pin, OUTPUT);
  Serial.begin(baud);
}

void loop() {

  digitalWrite(pin, HIGH);
  
  if (Serial.available() > 0) {
    incomingByte = "";
    incomingByte = Serial.readString(); 
  }
  
  if (incomingByte.length() > 0) {
    // Start Bit
    digitalWrite(pin, LOW);
    delay(8); 

    // Encoded Data
    for (int x = 0; x < incomingByte.length(); x++) {
      char a = incomingByte[x];
      const char *readBit = &a;
      if (strcmp(readBit, "1") == 0) {
        digitalWrite(pin, HIGH);
        delay(2);
        digitalWrite(pin, LOW);
        delay(1);
      }
      if (strcmp(readBit, "0") == 0) {
        digitalWrite(pin, HIGH);
        delay(1);
        digitalWrite(pin, LOW);
        delay(1);
      }
    }

    // Reset Buffer
    //incomingByte = "";
  }
}
