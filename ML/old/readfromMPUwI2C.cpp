#include "pico/stdlib.h"
#include "hardware/i2c.h"
#include <stdio.h>

// MPU6050 registers
#define MPU6050_ADDR       0x68
#define WHO_AM_I_REG       0x75
#define PWR_MGMT_1         0x6B
#define ACCEL_XOUT_H       0x3B

// Gesture sample count
#define GESTURE_LENGTH     100

// Structure to store gesture data
struct IMUReading {
    int16_t accel_x, accel_y, accel_z;
    int16_t gyro_x, gyro_y, gyro_z;
};

// Global gesture buffer
IMUReading gesture[GESTURE_LENGTH];

// I2C setup
void init_i2c() {
    i2c_init(i2c0, 400 * 1000); // 400kHz
    gpio_set_function(0, GPIO_FUNC_I2C);
    gpio_set_function(1, GPIO_FUNC_I2C);
    gpio_pull_up(0);
    gpio_pull_up(1);
}

// Initialize MPU6050
void init_mpu6050() {
    uint8_t data[2];
    data[0] = PWR_MGMT_1;
    data[1] = 0x00; // Wake up MPU6050
    i2c_write_blocking(i2c0, MPU6050_ADDR, data, 2, false);
}

// Read 14 bytes (Accel + Gyro)
bool read_imu(IMUReading& reading) {
    uint8_t reg = ACCEL_XOUT_H;
    uint8_t buffer[14];
    if (i2c_write_blocking(i2c0, MPU6050_ADDR, &reg, 1, true) < 0) return false;
    if (i2c_read_blocking(i2c0, MPU6050_ADDR, buffer, 14, false) < 0) return false;

    reading.accel_x = (buffer[0] << 8) | buffer[1];
    reading.accel_y = (buffer[2] << 8) | buffer[3];
    reading.accel_z = (buffer[4] << 8) | buffer[5];

    reading.gyro_x = (buffer[8] << 8) | buffer[9];
    reading.gyro_y = (buffer[10] << 8) | buffer[11];
    reading.gyro_z = (buffer[12] << 8) | buffer[13];

    return true;
}

// Main loop
int main() {
    stdio_init_all();
    init_i2c();
    init_mpu6050();

    printf("Starting gesture capture in 3 seconds...\n");
    sleep_ms(3000);

    for (int i = 0; i < GESTURE_LENGTH; i++) {
        if (read_imu(gesture[i])) {
            printf("Sample %d: AX=%d AY=%d AZ=%d GX=%d GY=%d GZ=%d\n", i,
                gesture[i].accel_x, gesture[i].accel_y, gesture[i].accel_z,
                gesture[i].gyro_x, gesture[i].gyro_y, gesture[i].gyro_z);
        }
        else {
            printf("Failed to read IMU at sample %d\n", i);
        }
        sleep_ms(50); // ~20Hz
    }

    printf("Gesture capture done!\n");

    // You can now store `gesture[]` or send it over UART for comparison.

    while (true) {
        tight_loop_contents();
    }

    return 0;
}
