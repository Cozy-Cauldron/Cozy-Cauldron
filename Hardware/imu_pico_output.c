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
#define SELECT_GPIO 10
#define INVENTORY_GPIO 11
#define JOYSTICK_SDA 6
#define JOYSTICK_SCL 7

// IMU devices on bus addr 0x68 by default
static int addr = 0x68;

// Joystick on bus addr 0x52 by default
static int joystick_addr = 0x52;

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
	gpio_init(SELECT_GPIO);
	gpio_init(INVENTORY_GPIO);

    sleep_ms(200);

    gpio_set_dir(BUTTON_GPIO, GPIO_IN);
    gpio_pull_up(BUTTON_GPIO);
	gpio_set_dir(SELECT_GPIO, GPIO_IN);
    gpio_pull_up(SELECT_GPIO);
	gpio_set_dir(INVENTORY_GPIO, GPIO_IN);
    gpio_pull_up(INVENTORY_GPIO);

    // IMU uses I2C0 on SDA (GPIO pin 4) and SCL (GPIO pin 5)
    i2c_init(i2c_default, 400 * 1000); // baud rate 400000
    gpio_set_function(PICO_DEFAULT_I2C_SDA_PIN, GPIO_FUNC_I2C);
    gpio_set_function(PICO_DEFAULT_I2C_SCL_PIN, GPIO_FUNC_I2C);
    gpio_pull_up(PICO_DEFAULT_I2C_SDA_PIN);
    gpio_pull_up(PICO_DEFAULT_I2C_SCL_PIN);
	
    // make the I2C pins available to picotool
    bi_decl(bi_2pins_with_func(PICO_DEFAULT_I2C_SDA_PIN, PICO_DEFAULT_I2C_SCL_PIN, GPIO_FUNC_I2C));

	// Joystick uses SDA (GPIO pin 6) and SCL (GPIO pin 7)
    i2c_init(i2c1, 400 * 1000); // baud rate 400000
    gpio_set_function(JOYSTICK_SDA, GPIO_FUNC_I2C);
    gpio_set_function(JOYSTICK_SCL, GPIO_FUNC_I2C);
    gpio_pull_up(JOYSTICK_SDA);
    gpio_pull_up(JOYSTICK_SCL);

    // make the I2C pins available to picotool
    bi_decl(bi_2pins_with_func(JOYSTICK_SDA, JOYSTICK_SCL, GPIO_FUNC_I2C));

    mpu6050_reset();

    int16_t acceleration[3], gyro[3];

	printf("\nEntered Program...\n");

    while (true) {

		// buttons pull DOWN when pressed
		// if (!gpio_get(SELECT_GPIO)) {
		//	sleep_ms(20);

			// check button is still actually pressed (not bouncing)
		//	if (!gpio_get(SELECT_GPIO)) {
		//		printf("E\n");
		//	}

			// make sure this button press is over before checking for another one
		//	sleep_ms(70);
		// }

		if (!gpio_get(INVENTORY_GPIO)) {
			sleep_ms(30);

			if (!gpio_get(INVENTORY_GPIO)) {
				printf("I\n");
			}

			sleep_ms(70);
		}

		// SELECT_GPIO used to be to press E. SELECT_GPIO is now the gesture button (press K)
		// Joystick Z-axis now used to press E
        if (!gpio_get(SELECT_GPIO)) {
            sleep_ms(20);

            if (!gpio_get(SELECT_GPIO)) {
				// IMU uses I2C0 on SDA (GPIO pin 4) and SCL (GPIO pin 5)
    			//i2c_init(i2c_default, 400 * 1000); // baud rate 400000
    			//gpio_set_function(PICO_DEFAULT_I2C_SDA_PIN, GPIO_FUNC_I2C);
    			//gpio_set_function(PICO_DEFAULT_I2C_SCL_PIN, GPIO_FUNC_I2C);
    			//gpio_pull_up(PICO_DEFAULT_I2C_SDA_PIN);
    			//gpio_pull_up(PICO_DEFAULT_I2C_SCL_PIN);
                //printf("\n**** Reading gesture... ****\n");
				printf("K\n");
            }

            // output imu data while pressed
            while (!gpio_get(SELECT_GPIO)) {
				//printf("K\n");  //*************TESTING***********
                // read raw accel and gyro data
                mpu6050_read_raw(acceleration, gyro);

                // convert raw accel. to m/s^2 (range = +/- 2g * 9.81)
                // 14500 gave more correct accel. of gravity than 16384 from datasheet
                float acc_x = (((acceleration[0] / 14500.0) * 9.81) / 100) * (-1);
                float acc_y = ((acceleration[1] / 14500.0) * 9.81) / 100;
                float acc_z = (((acceleration[2] / 14500.0) * 9.81) / 100) * (-1);

                // convert raw gyro. to deg/s (range = +/- 250deg/s)
                float gyro_x = (gyro[0] / 131.0) * (-1);
                float gyro_y = gyro[1] / 131.0;
                float gyro_z = (gyro[2] / 131.0) * (-1);

                printf("%f,%f,%f,%f,%f,%f\n", gyro_x, gyro_y, gyro_z, acc_x, acc_y, acc_z);
                //printf("Gyro. X = %f, Y = %f, Z = %f\n", gyro_x, gyro_y, gyro_z);
                // printf("\n");
				sleep_ms(200);
            }
			//printf("Acc. X = %f, Y = %f, Z = %f\n", acc_x, acc_y, acc_z);
            //printf("Gyro. X = %f, Y = %f, Z = %f\n", gyro_x, gyro_y, gyro_z);
			printf("STOP READ\n");

		}

		else {
			// Joystick uses SDA (GPIO pin 6) and SCL (GPIO pin 7)
			// i2c_init(i2c1, 400 * 1000); // baud rate 400000
			// gpio_set_function(JOYSTICK_SDA, GPIO_FUNC_I2C);
    		// gpio_set_function(JOYSTICK_SCL, GPIO_FUNC_I2C);
    		// gpio_pull_up(JOYSTICK_SDA);
    		// gpio_pull_up(JOYSTICK_SCL);

    		// make the I2C pins available to picotool
    		// bi_decl(bi_2pins_with_func(JOYSTICK_SDA, JOYSTICK_SCL, GPIO_FUNC_I2C));

			uint8_t joystick_buffer[2];
			//uint8_t joystick_x = -1;
			//uint8_t joystick_y = -1;
			uint8_t joystick_start_addr = 0x52;

			i2c_write_blocking(i2c1, joystick_addr, &joystick_start_addr, 1, true); // True to keep master control of bus
    		i2c_read_blocking(i2c1, joystick_addr, joystick_buffer, 3, false); // False - finished with the bus

			uint8_t joystick_x = joystick_buffer[0];
			uint8_t joystick_y = joystick_buffer[1];
			uint8_t joystick_z = joystick_buffer[2];

			// *** UNCOMMENT TO CONSTANT OUTPUT JOYSTICK DIRECTION ****
			//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
			
			// *** 		 Y ~255 => W
			// ***
			// *** X ~0 => A	 X ~255 => D
			// ***
			// ***		  Y ~0 => S 

			if (joystick_z == 1) {
                sleep_ms(20);

                if (joystick_z == 1) {
                    printf("E\n");
                    sleep_ms(70);
                }
            }
			else if (joystick_x >= 0 & joystick_x <= 96 & joystick_y >= 160 & joystick_y <= 255) {
                printf("SD\n");
                //printf("D\n");
            }
			else if (joystick_x >= 160 & joystick_x <= 255 & joystick_y >= 160 & joystick_y <= 255) {
                printf("SA\n");
                //printf("A\n");
            }
			else if (joystick_x >= 0 & joystick_x <= 96 & joystick_y >= 0 & joystick_y <= 96) {
                printf("WD\n");
                //printf("D\n");
            }
            else if (joystick_x >= 160 & joystick_x <= 255 & joystick_y >= 0 & joystick_y <= 96) {
                printf("WA\n");
                //printf("A\n");
            }
			else if (joystick_x >= 96 & joystick_x <= 160 & joystick_y >= 224 & joystick_y <= 255) {
				//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
				printf("S\n");
			}
			else if (joystick_x >= 0 & joystick_x <= 32 & joystick_y >= 96 & joystick_y <= 160) {
				//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
				printf("D\n");
			}
			else if (joystick_x >= 96 & joystick_x <= 160 & joystick_y >= 0 & joystick_y <= 32) {
				//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
				printf("W\n");
			}
			else if (joystick_x >= 224 & joystick_x <= 255 & joystick_y >= 96 & joystick_y <= 160) {
				//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
				printf("A\n");
			}	
			else if (joystick_x >= 96 & joystick_x <= 160 & joystick_y >= 96 & joystick_y <= 160) {
				//printf("Joystick: X = %d, Y = %d, Z = %d\n", joystick_x, joystick_y, joystick_z);
				printf("O\n");
			}
			//if (joystick_z == 1) {
			//	sleep_ms(20);
			//	if (joystick_z == 1) {
			//		printf("E\n");
			//		sleep_ms(70);
			//	}
			//}
		}

		// *** FOR TESTING IF IMU BUTTON NOT RESPONDING ***
		// if below message is outputting, check inits
		//
        // else {
        // 	sleep_ms(1000);
        // 	printf("\n**** Not not working ****\n");
        // 	sleep_ms(1000);
        // }

        sleep_ms(100);
    }
	

}
