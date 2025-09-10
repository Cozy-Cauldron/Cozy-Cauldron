#include <vector>
#include <string>

struct Gesture {
    std::string name;
    std::vector<float> data; // Store gesture data as a vector of floats (IMU data points)
};

std::vector<Gesture> gestures_data = {
    /* These are example actions - we plan to program ~5-10 actions, and each will have
    significantly more data once we perform each action with the remote. */

    {"open_door", {0x04, 0x05, 0x06, 0x07, 0x08}}, // Example data
    {"stir_pot", {0x0a, 0x0b, 0x0c, 0x0d, 0x0e}},  // Example data
    {"cast_line", {0x11, 0x22, 0x33, 0x44, 0x55}}  // Example data
};
