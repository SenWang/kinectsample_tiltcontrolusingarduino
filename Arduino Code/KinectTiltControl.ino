int led = 13;
int upBtn =9 ;
int upBtnVal = 0 ;
int resetBtn =6 ;
int resetBtnVal = 0 ;
int downBtn =3 ;
int downBtnVal = 0 ;


void setup()
{
  Serial.begin(9600) ;
  pinMode(led,OUTPUT) ;
  pinMode(upBtn,INPUT) ;
  pinMode(resetBtn,INPUT) ;
  pinMode(downBtn,INPUT) ;
}

void loop() 
{
  upBtnVal = digitalRead(upBtn) ;
  resetBtnVal = digitalRead(resetBtn) ;
  downBtnVal = digitalRead(downBtn) ;
  
  if(upBtnVal == HIGH)
    Serial.println("<UP>");
  else if(resetBtnVal == HIGH)
    Serial.println("<RESET>");
  else if(downBtnVal == HIGH)
    Serial.println("<DOWN>");
    
  delay(500) ;
}
  
