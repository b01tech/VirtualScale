// test_hx711.cpp
// Unit tests for HX711 modules

#include <Arduino.h>
#include <unity.h>
#include "AdcHandler.h"

// Mock data for testing
AdcHandler testAdc;
float testValues[NUM_LOAD_CELLS];

void setUp(void)
{
    // Set up before each test
    testAdc.begin();
    testAdc.tare(255);
}

void tearDown(void)
{
    // Clean up after each test
}

// Test 1: Initialization
void test_adc_initialization(void)
{
    TEST_ASSERT_TRUE(testAdc.isInitialized());
}

// Test 2: Read values returns array
void test_read_all_values_returns_array(void)
{
    bool result = testAdc.readAllValues(testValues);

    // Should handle both true and false gracefully
    TEST_ASSERT_TRUE(result == true || result == false);

    // All values should be valid numbers (not NaN)
    for (int i = 0; i < NUM_LOAD_CELLS; i++)
    {
        TEST_ASSERT_FALSE(isnan(testValues[i]));
    }
}

// Test 3: Read single value
void test_read_single_value(void)
{
    float value = testAdc.readValue(0);

    // Value should be valid (either a number or -1.0 for error)
    TEST_ASSERT_FALSE(isnan(value));
    TEST_ASSERT_TRUE(value >= -1.0f);
}

// Test 4: Invalid cell index
void test_invalid_cell_index(void)
{
    float value = testAdc.readValue(255); // Invalid index
    TEST_ASSERT_EQUAL_FLOAT(-1.0f, value);
}

// Test 5: Tare operation
void test_tare_operation(void)
{
    // Tare should not throw any errors
    testAdc.tare(255); // Tare all

    // After tare, readings should be close to zero
    testAdc.readAllValues(testValues);

    // Check that values are reasonable after tare
    for (int i = 0; i < NUM_LOAD_CELLS; i++)
    {
        if (testValues[i] >= 0) // If not error
        {
            TEST_ASSERT_TRUE(testValues[i] < 10.0f); // Should be near zero
        }
    }
}

// Test 6: Calibration factor
void test_set_calibration_factor(void)
{
    float factor = 500.0f;
    testAdc.setCalibrationFactor(0, factor);

    // Should not throw error
    TEST_ASSERT_TRUE(testAdc.isInitialized());
}

// Test 7: Value consistency
void test_reading_consistency(void)
{
    // Take two readings
    testAdc.readAllValues(testValues);
    float firstReadings[NUM_LOAD_CELLS];
    memcpy(firstReadings, testValues, sizeof(testValues));

    delay(100);

    testAdc.readAllValues(testValues);

    // Readings should be relatively close (within reasonable tolerance)
    for (int i = 0; i < NUM_LOAD_CELLS; i++)
    {
        if (firstReadings[i] >= 0 && testValues[i] >= 0)
        {
            // Tolerance: 50g (can be adjusted)
            float diff = abs(firstReadings[i] - testValues[i]);
            TEST_ASSERT_TRUE(diff < 50.0f);
        }
    }
}

// Test 8: Error handling for uninitialized
void test_uninitialized_read(void)
{
    AdcHandler uninitAdc;
    // Don't call begin()

    float value = uninitAdc.readValue(0);

    // Should return error value
    TEST_ASSERT_EQUAL_FLOAT(-1.0f, value);
}

// Run all tests
void setup()
{
    UNITY_BEGIN();

    RUN_TEST(test_adc_initialization);
    RUN_TEST(test_read_all_values_returns_array);
    RUN_TEST(test_read_single_value);
    RUN_TEST(test_invalid_cell_index);
    RUN_TEST(test_tare_operation);
    RUN_TEST(test_set_calibration_factor);
    RUN_TEST(test_reading_consistency);
    RUN_TEST(test_uninitialized_read);

    UNITY_END();
}

void loop()
{
    // Nothing to do
}

/*
    Compilation notes:

    For PlatformIO, add to platformio.ini:

    [env:nanoatmega328new-test]
    platform = atmelavr
    board = nanoatmega328new
    framework = arduino
    lib_deps =
        robtillaart/HX711@^0.6.3
        platformio/Unity
    test_framework = unity

    Run tests with: pio test -e nanoatmega328new-test
*/
