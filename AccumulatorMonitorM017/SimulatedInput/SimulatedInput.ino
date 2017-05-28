/*
Byte 1 = Segment ID, this will be an unsigned 8 bit integer ranging from 0-5
Byte 2 = Length of remaining message in bytes after this byte.
Byte 3-50 grouped in uint16, each one is 100* the cell voltage of cells 0-23
Byte 51-98 grouped in uint16, each one is the thermistor temperature *100 of thermistors 0-23
Byte 99-100 is a two byte checksum calculated using this function.
Byte 101-102 is the frame end 0xFFFE
*/

#define arrLen 102
#define randVolMin 300
#define randVolMax 420
#define randTempMin 500
#define randTempMax 4000

uint8_t sendArr[arrLen];

void setup() {
  // put your setup code here, to run once:
  Serial.begin(1000000);
  InitArr();
}

void loop() {
  // put your main code here, to run repeatedly:
  delay(2000);
  SetArr();
  Serial.write(sendArr, 102);
  
}

void InitArr() {
  for(int i = 0; i < arrLen; i++) {
    sendArr[i] = 0;
  }

  // length of remaining message does not change
  sendArr[1] = 100;

  for(int i = 2; i < 50; i = i + 2) {

    // generate a random number for each
    uint16_t vol = random(randVolMin,randVolMax);
    uint16_t temp = random(randTempMin,randTempMax);

    // set array elemnts
    sendArr[i] = lowByte(vol);
    sendArr[i + 1] = highByte(vol);

    sendArr[i + 48] = lowByte(temp);
    sendArr[i + 49] = highByte(temp);
    
  }

  // add the frameend
  sendArr[100] = 0xFF;
  sendArr[101] = 0xFE;

}

void SetArr() {
  
  // segment ID
  sendArr[0] = sendArr == 5 ? 0 : sendArr + 1;

  uint16_t chksm = 0;
  
  for(int i = 2; i < 50; i = i + 2) {

    // read elements from array
    uint16_t vol = sendArr[i] + sendArr[i+1] * 256;
    uint16_t temp = sendArr[i + 48] + sendArr[i + 49] * 256;

    // increment or reset elements
    vol = vol < randVolMax - 10 ? vol + 10 : vol - 110;
    temp = temp < randTempMax - 10 ? temp + 10 : temp - 3490;

    // set array elements
    sendArr[i] = lowByte(vol);
    sendArr[i + 1] = highByte(vol);
    
    sendArr[i + 48] = lowByte(temp);
    sendArr[i + 49] = highByte(temp);

    // add to checksum
    chksm += vol;
    chksm += temp;
    
  }

  // add the checksum
  sendArr[98] = lowByte(chksm);
  sendArr[99] = highByte(chksm);
  

  
}

