#uses imported classifier to easily go from controller->ML
#also includes basic ML->unity keystroke

import serial
import re
import numpy as np
from collections import deque
import keyboard 
import pyautogui
import time

#this is importing the ML algos, could also export using pickle
import joblib 

#config
SERIAL_PORT = 'COM9'
BAUD_RATE = 400000
FRAME_LEN = 6  # number of samples per inference
THRESHOLD = 0.6  # confidence cutoff for failed gestures
prediction_cnt = 0
gesture_cnt = 0
FEATURES = ["gyro_x", "gyro_y", "gyro_z", "acc_x", "acc_y", "acc_z"]

#load ML model
clf = joblib.load("beta_gesture_model.joblib")
scaler = joblib.load("beta_gesture_scaler.joblib")
encoder = joblib.load("beta_gesture_encoder.joblib")

def preprocess_gesture(frames, frame_len=FRAME_LEN):
    """
    Preprocess a gesture (list of [gyro_x, gyro_y, gyro_z, acc_x, acc_y, acc_z])
    by padding/truncating and flattening to match training input format.
    """
    data = np.array(frames)

    # pad/truncate
    if data.shape[0] < frame_len:
        pad_len = frame_len - data.shape[0]
        data = np.vstack([data, np.zeros((pad_len, data.shape[1]))])
    elif data.shape[0] > frame_len:
        data = data[:frame_len, :]

    # flatten and scale
    flat = data.flatten().reshape(1, -1)
    flat = scaler.transform(flat)
    return flat

#regex patterns
gyro_pattern = re.compile(r'Gyro\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')
acc_pattern  = re.compile(r'Acc\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')

#access serial connection
ser = serial.Serial(SERIAL_PORT, BAUD_RATE, timeout=1)

print("Streaming data... Press Ctrl+C to stop.")

try:
    while True:
        if gesture_cnt == 3:
            time.sleep(3)
            gesture_cnt = 0

        line = ser.readline().decode('utf-8', errors='ignore').strip()
        #print("**** LINE **** : ", line) #<=FOR TESTING
       
        #*** Adding stuff for other button/joystick inputs to keystrokes ***
        #currentDirection = 'o'

        if line == "WA" :
            print("Joystick moved forward and left (W+A)\n");
            pyautogui.keyDown('w')
            pyautogui.keyDown('a')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "WA" :
                continue
            pyautogui.keyUp('w')
            pyautogui.keyUp('a')
        elif line == "WD" :
            print("Joystick moved forward and right (W+D)\n");
            pyautogui.keyDown('w')
            pyautogui.keyDown('d')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "WD" :
                continue
            pyautogui.keyUp('w')
            pyautogui.keyUp('d')
        elif line == "SA" :
            print("Joystick moved down and left (S+A)\n");
            pyautogui.keyDown('s')
            pyautogui.keyDown('a')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "SA" :
                continue
            pyautogui.keyUp('s')
            pyautogui.keyUp('a')
        elif line == "SD" :
            print("Joystick moved down and right (S+D)\n");
            pyautogui.keyDown('s')
            pyautogui.keyDown('d')
            while ser.readline().decode('utf-8', errors='ignore').strip() == "SD" :
                continue
            pyautogui.keyUp('s')
            pyautogui.keyUp('d')
        elif line == "W" :
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
            pyautogui.press('e')

        elif line == "I" :
            print("I (Inventory) button pressed\n")
            pyautogui.press('i')

        elif line == "K" :
            stop_flag = False
            RECORD_DURATION = 7.0  # seconds
            start_time = time.time()
            buffer = []
            #if gesture_cnt == 3:
                #gesture_cnt = 0;
                #prediction_cnt = 0;

                #time.sleep(3)
                #continue
            
            while time.time() - start_time < RECORD_DURATION:
                    #if gesture_cnt == 3:
                    #    break
                    
                    if stop_flag == True:
                        break

                    if len(buffer) >= 6: 
                        # Stop collecting once we have enough frames
                        break
                    #imu_line = ser.readline().decode('utf-8', errors='ignore').strip()
                    #print(imu_line)
                    #parts = imu_line.split(",")
                    #ax, ay, az, gx, gy, gz = parts
                    #packet = [float(ax), float(ay), float(az), float(gx), float(gy), float(gz)]
                    #packet = imu_line.split(",")
                    #buffer.append(packet)
                    
                    print("adding to buffer")
                    print(buffer)
                    imu_line = ser.readline().decode('utf-8', errors='ignore').strip()
                    
                    if imu_line == "STOP READ":
                        stop_flag = True
                        break

                    buffer.append(imu_line)
                    
                    print(len(buffer))
                    print(buffer)
                    
                    #print(packet)

            if stop_flag == True:
                continue

            #if prediction_cnt == 0:
            #    filtered = [x for i, x in enumerate(buffer) if i % 2 == 1]
            #if prediction_cnt == 1:
            #    filtered = [x for i, x in enumerate(buffer) if i % 2 == 0]
            #print("AAAAAAHHHH")
            #print(filtered)
            #buffer = []
            #print(buffer)

            #filtered = [[float(x) for x in row] for row in filtered]
            #filtered = np.array(filtered, dtype=float)
            filtered = [list(map(float, row.split(','))) for row in buffer]
            print(filtered)
            user_data = np.array(filtered)

            print(user_data)

            X_new = preprocess_gesture(user_data)

            # Run prediction
            probs = clf.predict_proba(X_new)[0]
            max_prob = np.max(probs)
            pred_class = np.argmax(probs)

            THRESHOLD = 0.3
            if max_prob < THRESHOLD:
                gesture = "FAILED"
            else:
                gesture = encoder.inverse_transform([pred_class])[0]
                pyautogui.press(gesture)

            # Reset buffer between gestures
            line = "O"

            print(f"Predicted gesture: {gesture} | Confidence: {max_prob:.3f}")
            print("Class probabilities:")
            for cls, prob in zip(encoder.classes_, probs):
                print(f"  {cls}: {prob:.3f}")

            #prediction_cnt = 1
            #gesture_cnt += 1
            #if gesture_cnt == 3:
                #time.sleep(1)




       
except KeyboardInterrupt:
    print("\nStopped streaming.")
finally:
    ser.close()
