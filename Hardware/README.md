Pre-Alpha Hardware Build Placeholder Concepts

joystick class
- holds X/Y-axis data elements
- initialize & update functions
- sends elements to board via i2c/connection

IMU class
- holds accelerometer/gyrometer data elements
- initialize & update functions
- sends elements to board via i2c/connection

Pico Board
- component initialize & update functions
- sends received elements from board to connected device

Connected Device
- stores ML training dataset?
- receives info from Pico, processes data to check for valid gestures?
- identify (in)valid gestures, then send interpreted result to Game Software for future handling?
- assume more info in ML folder of repo