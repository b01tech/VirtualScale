#if !defined(GLOBAL_CONFIG_H)
#define GLOBAL_CONFIG_H

#include "Arduino.h"
#include "Hardware.h"

// loadcells configuration
#define NUM_LOAD_CELLS 4
#define CALIBRATION_FACTOR 1000.0f
#define NUM_READINGS 10

// serial communication
#define SERIAL_BAUD_RATE 115200

// delay in ms for the main loop
#define DELAY_IN_MS 200

#endif // GLOBAL_CONFIG_H
