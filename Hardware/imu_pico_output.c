/**
 * REFERENCES USED:
 *
 * https://github.com/raspberrypi/pico-examples
 *
 *
 * DEPENDENCIES:
 *
 * https://github.com/raspberrypi/pico-sdk
 *
 * https://github.com/raspberrypi/picotool
 * be sure to include libusb in install
 * be sure TinyUSB submodule initialized
 * 
 *
 * NOTES:
 *
 * Run serialToCSV.py to record data to motion_data.csv
 * If running py script in WSL2 (don't recommend) find serial port in Powershell & bind/attach via usbipd after boot
 *  
 */

#include <stdio.h>
#include "pico/stdlib.h"
#include "pico/binary_info.h"
#include "hardware/i2c.h"
#include "hardware/gpio.h"

#define BUTTON_GPIO 9

// IMU devices on bus addr 0x68 by default
static int addr = 0x68;

#ifdef i2c_default
static void mpu6050_reset() {
    // reset: first byte register, second byte data
    uint8_t buf[] = {0x6B, 0x80};
    i2c_write_blocking(i2c_default, addr, buf, 2, false);
    sleep_ms(100);
	
	// wake up
    buf[1] = 0x00;  // clear sleep mode: write 0x00 to register @ 0x6B
    i2c_write_blocking(i2c_default, addr, buf, 2, false);
    sleep_ms(10);
}

static void mpu6050_read_raw(int16_t accel[3], int16_t gyro[3]) {
	// register being read will auto increment
	// only need to send FIRST register we want to read, then read

	// 6 bytes for accel/gyro, 2 bytes each axis
    uint8_t buffer[6];

    // send 0x3B (first accel register) and start reading
    uint8_t val = 0x3B;
    i2c_write_blocking(i2c_default, addr, &val, 1, true); // true to keep master control of bus
    i2c_read_blocking(i2c_default, addr, buffer, 6, false);

    for (int i = 0; i < 3; i++) {
        accel[i] = (buffer[i * 2] << 8 | buffer[(i * 2) + 1]);
    }

    // send 0x43 (first gyro register) and start reading
    val = 0x43;
    i2c_write_blocking(i2c_default, addr, &val, 1, true);
    i2c_read_blocking(i2c_default, addr, buffer, 6, false);  // False - finished with bus

    for (int i = 0; i < 3; i++) {
        gyro[i] = (buffer[i * 2] << 8 | buffer[(i * 2) + 1]);;
    }
}
#endif

int main() {
    stdio_init_all();
    gpio_init(BUTTON_GPIO);

    sleep_ms(200);

    gpio_set_dir(BUTTON_GPIO, GPIO_IN);
    gpio_pull_up(BUTTON_GPIO);

    // uses I2C0 on SDA (GPIO pin 4) and SCL (GPIO pin 5)
    i2c_init(i2c_default, 400 * 1000); // baud rate 400000
    gpio_set_function(PICO_DEFAULT_I2C_SDA_PIN, GPIO_FUNC_I2C);
    gpio_set_function(PICO_DEFAULT_I2C_SCL_PIN, GPIO_FUNC_I2C);
    gpio_pull_up(PICO_DEFAULT_I2C_SDA_PIN);
    gpio_pull_up(PICO_DEFAULT_I2C_SCL_PIN);

    // make the I2C pins available to picotool
    bi_decl(bi_2pins_with_func(PICO_DEFAULT_I2C_SDA_PIN, PICO_DEFAULT_I2C_SCL_PIN, GPIO_FUNC_I2C));

    mpu6050_reset();

    int16_t acceleration[3], gyro[3];

    while (true) {

        // button pulls DOWN when pressed
        if (!gpio_get(BUTTON_GPIO)) {
            sleep_ms(50);

            // check button is still actually pressed (not bouncing)
            if (!gpio_get(BUTTON_GPIO)) {
                printf("\n**** Reading gesture... ****\n");
            }

            // output imu data while pressed
            while (!gpio_get(BUTTON_GPIO)) {
                // read raw accel and gyro data
                mpu6050_read_raw(acceleration, gyro);

                // convert raw accel. to m/s^2 (range = +/- 2g * 9.81)
                // 14500 gave more correct accel. of gravity than 16384 from datasheet
                float acc_x = (acceleration[0] / 14500.0) * 9.81;
                float acc_y = (acceleration[1] / 14500.0) * 9.81;
                float acc_z = (acceleration[2] / 14500.0) * 9.81;

                // convert raw gyro. to deg/s (range = +/- 250deg/s)
                float gyro_x = gyro[0] / 131.0;
                float gyro_y = gyro[1] / 131.0;
                float gyro_z = gyro[2] / 131.0;

                printf("Acc. X = %.2f, Y = %.2f, Z = %.2f\n", acc_x, acc_y, acc_z);
                printf("Gyro. X = %.2f, Y = %.2f, Z = %.2f\n", gyro_x, gyro_y, gyro_z);
                // printf("\n");
				sleep_ms(200);
            }

            printf("\n**** Stopped reading gesture ****\n");
        }

		// *** FOR TESTING IF BUTTON NOT RESPONDING ***
        // else {
        // 	sleep_ms(1000);
        // 	printf("\n**** Not not working ****\n");
        // 	sleep_ms(1000);
        // }

        sleep_ms(100);
    }

}
