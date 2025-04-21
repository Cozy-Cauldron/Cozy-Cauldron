The ML portion of this project is all about transforming the accelerometer, gyroscope, and IMU data into recognizable gestures that appear in-game. Because of this, ML is communicating with both the hardware and the software. It is receiving information from the hardware components, and sending information to the software. 

PRE-ALPHA:
- until the hardware components are solidified, only the basic functionality of the ML has been implemented. 
- the main idea of training gestures is to demo the action we want to program with the controller, then store this data as the gesture. For example, for opening a door, we will simulate the action of opening a door. Then, this data will be stored. When a game user performs an action with the remote, the user data will be compared to the data of the programmed gesture. ML will decide if the parsed user input is similar enough to the programmed gesture. 

KNOWN BUGS: 
- since gestures have not been pre-programmed, the user data cannot be compared 
- issues with reading from the csv file if the data is not in hex format

PROTOTYPE:
- added serial to csv conversion file, which reads from the accelerometer and gyroscope and formats the data into a csv file, which is needed for TensorFlow/ML operations.
- added basis for I squared C, may or may not be the direction we decide to go in, but will be capable of reading from controller directly to i2c
- working ML model that loads in training and testing data, and decides which "action" from the training data matches closest with testing data. 
- uses random forest classification for comparison
- uses PCA and radar plot for data visuals

KNOWN BUGS:
- currently experimenting with different sizes for csv file, I will test which sizes are optimal for feature extraction and selection.
- confusion matrix error: I believe it could be due to the dummy data being too similar to training data.
- some difficulty with handling labels, I may resort to changing the csv file format to include a gesture id so I can more easily debug this issue
