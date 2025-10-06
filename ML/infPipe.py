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
FRAME_LEN = 6  # number of samples per inference
THRESHOLD = 0.6  # confidence cutoff for failed gestures

#load ML model
clf = joblib.load("beta_gesture_model.joblib")
scaler = joblib.load("beta_gesture_scaler.joblib")
encoder = joblib.load("beta_gesture_encoder.joblib")

#regex patterns
gyro_pattern = re.compile(r'Gyro\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')
acc_pattern  = re.compile(r'Acc\. X = (-?[0-9]*[.]?[0-9]+), Y = (-?[0-9]*[.]?[0-9]+), Z = (-?[0-9]*[.]?[0-9]+)')

#access serial connection
ser = serial.Serial(SERIAL_PORT, BAUD_RATE, timeout=1)

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
            pyautogui.press('e')

        elif line == "I" :
            print("I (Inventory) button pressed\n")
            pyautogui.press('i')

        elif line == "K" :

            #steps:
            #data is coming in from com8 via serial data
            #keep collecting data for as long as "K" is being held down
            #once K is released, stop collecting data and send data to preprocessing
            #once data has been preprocessed, run the data through ml model
            #print the model's output

            print("Recording gesture data...")

            # Use a buffer to store all samples while K is held
            buffer = []

            # Keep reading until K is released
            while True:
                raw_line = ser.readline().decode('utf-8', errors='ignore').strip()
                
                # If button released (no longer "K"), break out
                if raw_line != "K":
                    print("K released, stopping recording.")
                    break

                # Read the next IMU data line
                imu_line = ser.readline().decode('utf-8', errors='ignore').strip()

                parts = imu_line.split(",")

                # Expecting: timestamp, ax, ay, az, gx, gy, gz
                if len(parts) < 7:
                    continue  # Skip malformed packets

                try:
                    timestamp, ax, ay, az, gx, gy, gz = parts
                    packet = [float(ax), float(ay), float(az), float(gx), float(gy), float(gz)]
                    buffer.append(packet)
                except ValueError:
                    continue  # Skip any non-numeric rows

            # Once recording ends, process the data if we have enough samples
            if len(buffer) == 0:
                print("No IMU data collected, skipping.")
                continue

            # Convert to NumPy array for preprocessing
            X_new = np.array(buffer)

            print("\n--- RAW DATA STATS ---")
            print(f"Raw shape (frames x features): {X_new.shape}")
            print(f"Expected FRAME_LEN: {FRAME_LEN}")
            print("First 2 raw frames (ax, ay, az, gx, gy, gz):")
            print(X_new[:2])

            # Preprocessing step
            if X_new.shape[0] < FRAME_LEN:
                pad_len = FRAME_LEN - X_new.shape[0]
                print(f"Padding with {pad_len} zero-rows to reach {FRAME_LEN} frames.")
                X_new = np.vstack([X_new, np.zeros((pad_len, X_new.shape[1]))])
            elif X_new.shape[0] > FRAME_LEN:
                print(f"Truncating to first {FRAME_LEN} frames.")
                X_new = X_new[:FRAME_LEN, :]

            print(f"Adjusted shape (after pad/truncate): {X_new.shape}")
            
            # Flatten the sequence or scale as your model expects
            X_scaled = scaler.transform(X_new)
            print("\n--- AFTER SCALING ---")
            print(f"Scaled data shape: {X_scaled.shape}")
            print("First 2 scaled frames:")
            print(X_scaled[:2])

            
            X_flattened = X_scaled.flatten().reshape(1, -1)
            print("\n--- AFTER FLATTENING ---")
            print(f"Flattened shape: {X_flattened.shape}")
            print(f"First 12 flattened values: {X_flattened[0][:12]}")
            print("-----------------------\n")
            
            # Run the ML model
            y_pred_proba = clf.predict_proba(X_flattened)[0]
            y_pred_index = np.argmax(y_pred_proba)
            y_pred_label = encoder.inverse_transform([y_pred_index])[0]
            confidence = y_pred_proba[y_pred_index]

            # Output prediction or failed gesture
            if confidence >= THRESHOLD:
                print(f"Predicted Gesture: {y_pred_label} (confidence {confidence:.2f})")
                pyautogui.press(y_pred_label)
            else:
                print(f"Gesture unclear. Confidence {confidence:.2f} < {THRESHOLD}. Failed trace detected.")


       
except KeyboardInterrupt:
    print("\nStopped streaming.")
finally:
    ser.close()