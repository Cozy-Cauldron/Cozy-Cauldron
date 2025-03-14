#include "IMU.h"

// Default constructor
IMUclass::IMUclass(void) {
    accelX = 0;
    accelY = 0;
    accelZ = 0;

    gyroX = 0;
    gyroY = 0;
    gyroZ = 0;
}

// initializer
bool IMUclass::init(void){
    // initialize stdio
    // initialize i2c

    // set functions & pull up pins
    // set ctrl registers to wanted settings using set_reg

    return true;
}

// reset component
bool IMUclass::reset(void){
    // reset functions, pull down pins
    // reset ctrl registers 

    accelX = 0;
    accelY = 0;
    accelZ = 0;

    gyroX = 0;
    gyroY = 0;
    gyroZ = 0;

    init();
    return true;
}

// update data elements
void IMUclass::update(void){
    // placeholders from LIS3DH
    uint8_t lsBuffer, msBuffer;
    uint16_t fullBuffer;

    /*
    // A = X, Y, Z
    // B = Accel, Gyro
    // Refers to defined addr of reg

    lsBuffer = read_reg(AL_B);
    msBuffer = read_reg(AH_B);

    // placeholder from embedded drivers
    // retool later to fit IMU, mode, and resolution
    fullBuffer = (lsBuffer >> 6 | msBuffer << 2);
    if ((fullBuffer >> 9) == 1){ // Negative
        B_A = -(fullBuffer - 1024) * 4 / 100;
    }
    else{ // Positive
        B_A = (fullBuffer - 1024) * 4 / 100;
    }
    */

    return;
}

// get functions
float IMUclass::getAccelX(void){
    return accelX;
}
float IMUclass::getAccelY(void){
    return accelY;
}
float IMUclass::getAccelZ(void){
    return accelZ;
}
float IMUclass::getGyroX(void){
    return gyroX;
}
float IMUclass::getGyroY(void){
    return gyroY;
}
float IMUclass::getGyroZ(void){
    return gyroZ;
}

// helper functions
void IMUclass::set_reg(uint8_t reg, uint8_t val){
    // i2c_write_blocking(I2C_PORT, reg, &val, 1, true);
    return;
}

// Read/Return byte at addr reg on accelerometer
uint8_t IMUclass::read_reg(uint8_t reg){
   uint8_t buffer;
   // i2c_write_blocking(I2C_PORT, reg, &reg, 1, true);
   // i2c_read_blocking(I2C_PORT, reg, &buffer, 1, false);
   return buffer;
}