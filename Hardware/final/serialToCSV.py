import serial
import re
import csv
from datetime import datetime

#config with controller
SERIAL_PORT = 'COM9'
BAUD_RATE = 400000
OUTPUT_CSV = 'final_gestures.csv'

#regex patterns for gyro and acc lines
full_pattern = re.compile(r'(-?[0-9]*[.]?[0-9]+),(-?[0-9]*[.]?[0-9]+),(-?[0-9]*[.]?[0-9]+),(-?[0-9]*[.]?[0-9]+),(-?[0-9]*[.]?[0-9]+),(-?[0-9]*[.]?[0-9]+)')
#gyro_pattern  = re.compile(r'(-?[0-9]*[.]?[0-9]+), (-?[0-9]*[.]?[0-9]+), (-?[0-9]*[.]?[0-9]+)')
#acc_pattern  = re.compile(r'(-?[0-9]*[.]?[0-9]+), (-?[0-9]*[.]?[0-9]+), (-?[0-9]*[.]?[0-9]+)')

#access serial connection
ser = serial.Serial(SERIAL_PORT, BAUD_RATE, timeout=1)

#prep CSV
with open(OUTPUT_CSV, 'w', newline='') as csvfile:
    fieldnames = ['timestamp', 'gyro_x', 'gyro_y', 'gyro_z', 'acc_x', 'acc_y', 'acc_z']
    writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
    writer.writeheader()

    print("Logging data. Press Ctrl+C to stop...")

    try:
        while True:
            line = ser.readline().decode('utf-8', errors='ignore').strip()
            #print(line)
            timestamp = datetime.now().isoformat()

            #gyro_match = gyro_pattern.match(line)
            pattern_match = full_pattern.match(line)
            if pattern_match:
                #gyro_data = list(map(float, gyro_match.groups()))
                pattern_data = list(map(float, pattern_match.groups()))
                # read the next 2 lines for Acc and Temp
                #acc_line = ser.readline().decode('utf-8', errors='ignore').strip()
                #acc_match = acc_pattern.match(acc_line)

                #if acc_match:
                #    acc_data = list(map(float, acc_match.groups()))

                writer.writerow({
                    'timestamp': timestamp,
                    'gyro_x': pattern_data[0],
                    'gyro_y': pattern_data[1],
                    'gyro_z': pattern_data[2],
                    'acc_x': pattern_data[3],
                    'acc_y': pattern_data[4],
                    'acc_z': pattern_data[5],
                })
                print(f"Recorded: ", pattern_data[0], pattern_data[1], pattern_data[2], pattern_data[3], pattern_data[4], pattern_data[5])
    except KeyboardInterrupt:
        print("\nStopped logging.")
    finally:
        ser.close()
