// Placeholder code before I can properly get my hands on the thing
// Probably connected to Pico through GPIO?

#include <stdint.h>

#include "hardware/gpio.h"
#include "hardware/adc.h"

class Joystick {
private:
// Private data elements for security?
    float xAxis, yAxis;
    bool zButtonPressed;

public:
// Default Constructor
// Probably set data elements to 0
    Joystick(void);

// initializer
// most likely setting up power/pulling pins
    bool init(void);

// Reset joystick 
// Pull pin opposite, reset values, reinit
    bool reset(void);

// Updates class elements with recorded values
// Remember to convert from 0~1024 (w/ mid being 512) to float
    void update(void);

// GetVal functions for private elements
    float getAxisX(void);
    float getAxisY(void);
    bool getZButton(void);
};