#uses imported classifier to easily go from controller->ML
#also includes basic ML->unity keystroke

import serial
import re
import numpy as np
from collections import deque
import keyboard 
import pyautogui

#this is importing the ML algos, could also export using pickle
import joblib 

#config
SERIAL_PORT = 'COM8'
BAUD_RATE = 400000
WINDOW_SIZE = 50  # number of samples per inference
MODEL_PATH = "gesture_classifier.pkl"  

#load ML model
clf, le = joblib.load(MODEL_PATH)

#regex patterns
gyro_pattern = re.compile(r'Gyro\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')
acc_pattern  = re.compile(r'Acc\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')

#access serial connection
ser = serial.Serial(SERIAL_PORT, BAUD_RATE, timeout=1)

#buffer for real-time pipeline
buffer = deque(maxlen=WINDOW_SIZE)

print("Streaming data... Press Ctrl+C to stop.")

try:
    while True:
        line = ser.readline().decode('utf-8', errors='ignore').strip()
        print(line) #<=FOR TESTING
       
        #*** Adding stuff for other button/joystick inputs to keystrokes ***
        currentDirection = 'o'

        if line == "W" :
            print("Joystick moved forward (W)\n")
            #currentDirection = 'w'
            pyautogui.keyDown('w')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "W" :
                continue
            pyautogui.keyUp('w')

        elif line == "A" :
            print("Joystick moved left (A)\n")
            #currentDirection = 'a'
            pyautogui.keyDown('a')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "A" :
                continue
            pyautogui.keyUp('a')

        elif line == "S" :
            print("Joystick moved back (S)\n")
            #currentDirection = 's'
            pyautogui.keyDown('s')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "S" :
                continue
            pyautogui.keyUp('s')

        elif line == "D" :
            print("Joystick moved right (D)\n")
            #currentDirection = 'd'
            pyautogui.keyDown('d')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "D" :
                continue
            pyautogui.keyUp('d')

        #elif line == "O" :
            #print("Joystick moved to origin\n")
            #if currentDirection != 'o' :
            #    pyautogui.keyUp(currentDirection)

        elif line == "E" :
            print("E (Select) button pressed\n")
            #keyboard.write("e")
            pyautogui.press('e')

        elif line == "I" :
            print("I (Inventory) button pressed\n")
            #keyboard.write("i")
            pyautogui.press('i')

        else :
            gyro_match = gyro_pattern.match(line)
            if gyro_match:
                print("Gyro recognized...\n")
                gyro_data = list(map(float, gyro_match.groups()))
                
                acc_line = ser.readline().decode('utf-8', errors='ignore').strip()
                acc_match = acc_pattern.match(acc_line)

                if acc_match:
                    print("Accel recognized...\n")
                    acc_data = list(map(float, acc_match.groups()))
                    sample = gyro_data + acc_data  # [gx, gy, gz, ax, ay, az]
                    buffer.append(sample)

                    #once we have enough samples, run inference
                    if len(buffer) == WINDOW_SIZE:
                        window = np.array(buffer)
                        # TODO: apply preprocessing here
                        features = extract_features(window)  
                        prediction = model.predict([features])[0]
                        confidence = max(model.predict_proba([features])[0])

                        if confidence > 0.7:
                            print("Predicted action:", prediction)
                    
                        # send prediction to unity (thanks kylie)
                        # Map gestures to keys
                        mapping = {
                            "zigzag": "z",
                            "fish": "x",
                            "clockwisecircle": "c",
                            "downcarrot": "v",
                            "jump": "j"
                        }

                        if predicted_label in mapping:
                            mapped_key = mapping[predicted_label]
                            print(f"Sending keystroke: {mapped_key}")
                            keyboard.write(mapped_key)  # types into the active window
    
                        else:
                            print("No confident action detected")
except KeyboardInterrupt:
    print("\nStopped streaming.")
finally:
    ser.close()
