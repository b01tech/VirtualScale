#include "GlobalConfiguration.h"
#include "AdcHandler.h"
#include "SerialHandler.h"

AdcHandler adcHandler;
SerialHandler serialHandler;

// Array to store load cell raw values
long loadCellValues[NUM_LOAD_CELLS];

void setup()
{
  serialHandler.begin();
  delay(500);

  adcHandler.begin();
}

void loop()
{
  adcHandler.printValues();
  // Wait before next reading
  delay(DELAY_IN_MS);
}
