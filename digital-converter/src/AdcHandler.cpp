#include "AdcHandler.h"

AdcHandler::AdcHandler() : initialized(false)
{
}

void AdcHandler::begin()
{
    loadcell1.begin(LOADCELL_DT1, LOADCELL_SCK);
    loadcell1.set_gain(128);
    delay(100);

    loadcell2.begin(LOADCELL_DT2, LOADCELL_SCK);
    loadcell2.set_gain(128);
    delay(100);

    loadcell3.begin(LOADCELL_DT3, LOADCELL_SCK);
    loadcell3.set_gain(128);
    delay(100);

    loadcell4.begin(LOADCELL_DT4, LOADCELL_SCK);
    loadcell4.set_gain(128);
    delay(100);

    initialized = true;
}

bool AdcHandler::isInitialized() const
{
    return initialized;
}

float AdcHandler::readValue(uint8_t cellIndex)
{
    switch (cellIndex)
    {
    case 1:
        return loadcell1.get_units(5);
    case 2:
        return loadcell2.get_units(5);
    case 3:
        return loadcell3.get_units(5);
    case 4:
        return loadcell4.get_units(5);

    default:
        break;
    }
    return 0.0f;
}

void AdcHandler::printValues()
{
    SerialHandler::sendLoadCellValue(1, readValue(1));
    SerialHandler::sendLoadCellValue(2, readValue(2));
    SerialHandler::sendLoadCellValue(3, readValue(3));
    SerialHandler::sendLoadCellValue(4, readValue(4));
}
