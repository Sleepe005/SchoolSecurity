// Подключение датчиков
const int tempPin = 0;
const int MQ2Pin = 2;
const int flamePin = 1;

const int buzzPin = 6;
const int ledPin = 7;

// Переменные для хранения данных с датчиков
double tempValue = 0;
double voltage = 0;

int MQ2Value = 0;

int flameValue = 0;

unsigned int place = 2;

bool haveFire = false;

void setup() {
  Serial.begin(9600);
  pinMode(flamePin, INPUT);
  analogWrite(flamePin, LOW);
  pinMode(ledPin, OUTPUT);
  pinMode(buzzPin, OUTPUT);
}

void loop() {
  // Получение значения температуры
  tempValue = analogRead(tempPin);
  voltage = (tempValue / 1023) * 5000;
  tempValue = voltage / 10;

  // Получения значения с датчика газа
  MQ2Value = analogRead(MQ2Pin);

  // Получение значения с датчика пламени
  flameValue = analogRead(flamePin);

  Serial.println("[StartData]");
  Serial.println(place);
  Serial.println((int)tempValue);
  Serial.println(flameValue);
  Serial.println(MQ2Value);
  Serial.println("[StopData]");

  if (Serial.available() > 0) {
    haveFire = true;
  } else {
    haveFire = false;
  }

  if (haveFire) {
    digitalWrite(ledPin, HIGH);
    tone(buzzPin, 5000, 1000);
    Serial.print("Yes");
    Serial.println(Serial.available());
  }

  delay(1000);
}