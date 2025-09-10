#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <cmath>
#include <string>
#include "gestures_data.h"
#include "gestures_recognition.cpp"
#include "error_detection.cpp"

int main() {
    //load user gesture data from a CSV file
    std::string userfile = "user_gesture_data.csv";
    std::vector<float> user_data = load_user_data(userfile);

    //check for faulty data (all 0x00 or all 0xFF)
    if (detect_faulty_data(user_data)) {
        std::cout << "Error: The input data is faulty (all 0x00 or all 0xFF).\n";
        return 1;
    }

    //compare the user data with pre-programmed gestures
    std::string match = compare_gesture_data(user_data);

    //output the result
    std::cout << "Best match: " << match << std::endl;

    return 0;
}