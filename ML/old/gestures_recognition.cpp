#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <cmath>
#include <string>
#include <limits>
#include "gestures_data.h"
#include "error_detection.cpp"

//compute Euclidean distance between two gesture data vectors (For now, just for testing)
float compute_euclidean_distance(const std::vector<float>& vec1, const std::vector<float>& vec2) {
    float sum = 0.0;
    for (size_t i = 0; i < vec1.size(); ++i) {
        sum += std::pow(vec1[i] - vec2[i], 2);
    }
    return std::sqrt(sum);
}

//load user gesture data (for now using csv file)
std::vector<float> load_user_data(const std::string& userfile) {
    std::vector<float> data;
    std::ifstream file(userfile);
    std::string line;

    if (file.is_open()) {
        while (std::getline(file, line)) {
            std::stringstream ss(line);
            std::string value;
            while (std::getline(ss, value, ',')) {
                data.push_back(hex_to_int(value));
            }
        }
        file.close();
    }
    return data;
}

//compare user gesture data to pre-programmed gestures
std::string compare_gesture_data(const std::vector<float>& user_data) {
    std::string best_match = "No match found";
    float min_distance = std::numeric_limits<float>::max();

    for (const auto& gesture : user_data) {
        //this section will be worked on the most; replace with ML algos
        float distance = compute_euclidean_distance(user_data, user_data);

        if (distance < min_distance) {
            min_distance = distance;
            best_match = "open_door";
        }
    }

    //return the match in the form of a string, for example, "cast_line"
    return best_match;
}
