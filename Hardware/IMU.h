// Placeholder code based off of LIS3DH from Embedded Drivers Lab
// Look into owned hardware later to retool towards specifications

#include <stdint.h>

class IMUclass {
private:
// Data values private for more security?
    // Represents accelerometer axes
    float accelX, accelY, accelZ;

    // Represents gyrometer axes
    float gyroX, gyroY, gyroZ;

public:
// Default Constructor
// Probably defaults all class elements to 0;
    IMUclass(void);

// Initialize IMU
// Use I2C to send setting values into IMU ctrl registers
// Aka use set_reg helper function
    bool init(void);

// Reset IMU
// Turn off, reset setting values, reset data elements to 0, then reinit?
    bool reset(void);

// Updates class elements with recorded values
// Use read_reg helper function to do so
// Remember to convert from the two's complement into the float
    void update(void);

// GetVal functions for private elements
// Use read_reg helper function for values
    float getAccelX(void);
    float getAccelY(void);
    float getAccelZ(void);
    float getGyroX(void);
    float getGyroY(void);
    float getGyroZ(void);

// Helper Functions
    // Use I2C to set reg at addr reg w/ data val
    void set_reg(uint8_t reg, uint8_t val);

    // Use I2C to read reg at addr reg and return held value
    uint8_t read_reg(uint8_t reg);
};