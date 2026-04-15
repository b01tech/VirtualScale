#if !defined(SERIALHANDLER_H)
#define SERIALHANDLER_H

#include "GlobalConfiguration.h"

#define NUM_LOAD_CELLS 4

// Format per line: id:value\r\n
// Example: 1:126032,00\r\n

class SerialHandler
{
private:
    char messageBuffer[32];

public:
    SerialHandler();
    void begin();
    static void sendLoadCellValue(uint8_t cellId, float value);
};

#endif // SERIALHANDLER_H
