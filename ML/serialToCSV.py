import serial
import re
import csv
from datetime import datetime

#config with controller
SERIAL_PORT = 'COM8'
BAUD_RATE = 9600
OUTPUT_CSV = 'motion_data.csv'

#regex patterns for gyro and acc lines
gyro_pattern = re.compile(r'Gyro\. X = (-?\d+), Y = (-?\d+), Z = (-?\d+)')
acc_pattern  = re.compile(r'Acc\. X = (-?\d+), Y = (-?\d+), Z = (-?\d+)')

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
            timestamp = datetime.now().isoformat()

            gyro_match = gyro_pattern.match(line)
            if gyro_match:
                gyro_data = list(map(int, gyro_match.groups()))
                #read the next 2 lines for Acc and Temp
                acc_line = ser.readline().decode('utf-8', errors='ignore').strip()
                acc_match = acc_pattern.match(acc_line)

                if acc_match:
                    acc_data = list(map(int, acc_match.groups()))

                    writer.writerow({
                        'timestamp': timestamp,
                        'gyro_x': gyro_data[0],
                        'gyro_y': gyro_data[1],
                        'gyro_z': gyro_data[2],
                        'acc_x': acc_data[0],
                        'acc_y': acc_data[1],
                        'acc_z': acc_data[2],
                    })
                    print(f"Recorded: G({gyro_data}) A({acc_data})")
    except KeyboardInterrupt:
        print("\nStopped logging.")
    finally:
        ser.close()
