#include "SerialHandler.h"

SerialHandler::SerialHandler()
{
    memset(messageBuffer, 0, sizeof(messageBuffer));
}

void SerialHandler::begin()
{
    Serial.begin(SERIAL_BAUD_RATE);
    delay(100);
}

void SerialHandler::sendLoadCellValue(uint8_t cellId, float value)
{
    // Format: id:value,00\r\n
    // Example: 1:126032,00
    Serial.print(cellId);
    Serial.print(":");
    Serial.print(value, 2);
    Serial.print("\r\n");
    delay(5);
}
