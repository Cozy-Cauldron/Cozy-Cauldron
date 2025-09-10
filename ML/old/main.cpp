#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <cmath>
#include <string>
#include <unordered_map>
#include <windows.h>

#include "gestures_data.h"
#include "gestures_recognition.cpp"
#include "error_detection.cpp"

// Send a single character as a keystroke
void sendKeystroke(char key) {
    INPUT input;
    input.type = INPUT_KEYBOARD;
    input.ki.wScan = 0;
    input.ki.time = 0;
    input.ki.dwExtraInfo = 0;

    // Key down
    input.ki.wVk = VkKeyScan(key);
    input.ki.dwFlags = 0;
    SendInput(1, &input, sizeof(INPUT));

    // Key up
    input.ki.dwFlags = KEYEVENTF_KEYUP;
    SendInput(1, &input, sizeof(INPUT));
}

int main() {
    // Load user gesture data
    std::string userfile = "user_gesture_data.csv";
    std::vector<float> user_data = load_user_data(userfile);

    // Check for faulty data
    if (detect_faulty_data(user_data)) {
        std::cout << "Error: The input data is faulty (all 0x00 or all 0xFF).\n";
        return 1;
    }

    // Compare gesture
    std::string match = compare_gesture_data(user_data);
    std::cout << "Best match: " << match << std::endl;

    // Map gesture string to keystroke
    std::unordered_map<std::string, char> mapping = {
        {"zigzag", 'z'},
        {"fish", 'x'},
        {"clockwisecircle", 'c'},
        {"downcarrot", 'v'}
    };

    if (mapping.find(match) != mapping.end()) {
        char mappedKey = mapping[match];
        std::cout << "Sending keystroke: " << mappedKey << std::endl;
        sendKeystroke(mappedKey);
    } else {
        std::cout << "No mapping for: " << match << std::endl;
    }

    return 0;
}
