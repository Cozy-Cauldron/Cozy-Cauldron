#include <vector>
#include <iostream>

//detect faulty data (all 0x00 or all 0xFF)
bool detect_faulty_data(const std::vector<float>& user_data) {
    // Check if all data points are 0x00
    bool all_zero = true;
    bool all_ff = true;

    for (const float& value : user_data) {
        //if any value is not 0, set all_zero to false
        if (value != 0) {
            all_zero = false;
        }

        //if any value is not 0xFF (which would be represented as 255.0 in float), set all_ff to false
        if (value != 255.0) {
            all_ff = false;
        }
    }

    //return true if the data is either all 0x00 or all 0xFF
    return all_zero || all_ff;
}

//convert a hex string to an integer
int hex_to_int(const std::string& hex_str) {
    int value;
    std::stringstream ss;
    ss << std::hex << hex_str;
    ss >> value;
    return value;
}