The hardware part of this project is based around getting the button, joystick, and IMU components set up, detecting data, and then transmitting said data to the PicoBoard to send out to the rest of the project for interpretation. As such, it primarily deals with the external interfaces that connect to the persistent state of the project.

Pre-Alpha:
- As hardware components are not yet set in stone, most code exists as prototypes to be retooled later for the specific values and locations of each module.
- The primary concept is splitting up each module into individual driver classes with their own private data and public functions. These will hold the recorded data from the external interfaces, and then send out that data to the rest of the project to be interpreted.

Fixed Bugs:
- Most code is non-functional, being prototype examples of the driver classes
  - C code for IMU communication and logging now works (goal is to convert/integrate into C++)
- PuTTY was used to demonstrate data could be sent over USB, but it cannot be used to display I2C data
  - Fixed dependency issues with libusb and TinyUSB; I2C data can be displayed serially with PuTTY and in the CSV

Current Bugs (See also: Issues):
- The IMU class (IMU.cpp/h) is based around LIS3DH code, but needs to be retooled properly for the different IMU spec we're using
- imu_pico_output.c should be changed to compile as C++ so driver classes can be fully integrated to organize tasks
