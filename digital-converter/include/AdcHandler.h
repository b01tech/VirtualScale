#if !defined(ADCHANDLER_H)
#define ADCHANDLER_H

#include "GlobalConfiguration.h"
#include "SerialHandler.h"
#include "HX711.h"

class AdcHandler
{
private:
    HX711 loadcell1;
    HX711 loadcell2;
    HX711 loadcell3;
    HX711 loadcell4;
    bool initialized;

public:
    AdcHandler();
    void begin();
    bool isInitialized() const;
    float readValue(uint8_t cellIndex);
    void printValues();
};

#endif // ADCHANDLER_H
