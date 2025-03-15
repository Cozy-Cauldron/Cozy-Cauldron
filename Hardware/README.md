The hardware part of this project is based around getting the button, joystick, and IMU components set up, detecting data, and then transmitting said data to the PicoBoard to send out to the rest of the project for interpretation. As such, it primarily deals with the external interfaces that connect to the persistent state of the project.

Pre-Alpha:
- As hardware components are not yet set in stone, most code exists as prototypes to be retooled later for the specific values and locations of each module.
- The primary concept is splitting up each module into individual driver classes with their own private data and public functions. These will hold the recorded data from the external interfaces, and then send out that data to the rest of the project to be interpreted.

Current Bugs (See also: Issues):
- Most code is non-functional, being prototype examples of the driver classes
- The IMU class is based around LIS3DH code, but needs to be retooled properly for the different IMU spec we're using
- Driver classes are lacking a main/board file that represents the PicoBoard receiving the data to send out to the other internal systems/persistent states.
- PuTTY was used to demonstrate data could be sent over USB, but it cannot be used to display I2C data. Another method will be needed to further test/debug the I2C communication between the IMU and Pico board.
