The hardware part of this project is based around getting the button, joystick, and IMU components set up, detecting data, and then transmitting said data to the PicoBoard to send out to the rest of the project for interpretation. As such, it primarily deals with the external interfaces that connect to the persistent state of the project.

Pre-Alpha:
- As hardware components are not yet set in stone, most code exists as prototypes to be retooled later for the specific values and locations of each module.
- The primary concept is splitting up each module into individual driver classes with their own private data and public functions. These will hold the recorded data from the external interfaces, and then send out that data to the rest of the project to be interpreted.

Prototype:
- Hardware components getting solidified, currently using GPIO buttons & joystick
- Code readjusted and rewritten into a test Pico 1 W project
- Driver classes are no longer pseudo, ideally should work with the Joystick/Button peripherals put into board
- Joystick uses Pico 1 W's ADC pins, which are 12-bit (0~4095)
- Main/board file has looped demo code for outputing Joystick X/Y-Axis numbers & direction + Z Button, and the A/B Buttons states

Current Bugs (See also: Issues):
- Main/board file currently has no actual code making use of the Driver libraries beyond initialization and currently is not connected to rest of project yet
- Joystick/Button updates have to be done manually, look later into updating them more actively in real-time as will be required for the final product. ADC+GPIO Interrupts/IRQs perhaps?
- Joystick & Button pins have placeholder PIN values for now. Update with actual values once known.

- PuTTY was used to demonstrate data could be sent over USB, but it cannot be used to display I2C data. Another method will be needed to further test/debug the I2C communication between the IMU and Pico board.