#include "Buttons.h"

#define AButtonPin 36
#define BButtonPin 35

// Default Constructor
Buttons::Buttons(void){
    buttAPressed = false;
    buttBPressed = false;
}

// initializer
bool Buttons::init(void){
    // init GPIO Pins
    gpio_init(AButtonPin);
    gpio_init(BButtonPin);
    // ENable GPIO Input
    gpio_set_input_enabled(AButtonPin, true);
    gpio_set_input_enabled(BButtonPin, true);

    // Pull pins up (Pressed when Low)
    gpio_pull_up(AButtonPin);
    gpio_pull_up(BButtonPin);

    return true;
}

// reset component
bool Buttons::reset(void){
    gpio_deinit(AButtonPin);
    gpio_deinit(BButtonPin);

    buttAPressed = false;
    buttBPressed = false;

    init();
    return true;
}

// update elements
void Buttons::update(void){
    // Code to debounce later?
    buttAPressed = !(gpio_get(AButtonPin));
    buttBPressed = !(gpio_get(BButtonPin));
    return;
}

// get functions
bool Buttons::getButtA(void){
    return buttAPressed;
}
bool Buttons::getButtB(void){
    return buttBPressed;
}