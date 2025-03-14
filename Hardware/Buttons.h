// Placeholder code before I can properly get my hands on the thing
// Probably connected to Pico through GPIO?

#include <stdint.h>

class Buttons {
private:
// Private data elements for security?
// True if pressed, False if not pressed
// May change to uint if we use pressure-sensitive buttons?
    bool buttA, buttB;

public:
// Default Constructor
// Probably set data elements to false
    Buttons(void);

// initializer
// most likely setting up power/pulling pins
    bool init(void);

// Reset  
// Pull pin opposite, reset values, reinit
    bool reset(void);

// Updates class elements with recorded values
    void update(void);

// GetVal functions for private elements
    float getButtA(void);
    float getButtB(void);
};