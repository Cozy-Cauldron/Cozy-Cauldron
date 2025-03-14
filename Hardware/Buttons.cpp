#include "Buttons.h"

// Default Constructor
Buttons::Buttons(void){
    buttA = false;
    buttB = false;
}

// initializer
bool Buttons::init(void){
    // pull pin to enable buttons?
    // give power to enable buttons
    return true;
}

// reset component
bool Buttons::reset(void){
    // pull pin back to disable
    // take off power to enable buttons

    buttA = 0;
    buttB = 0;

    init();
    return true;
}

// update elements
void Buttons::update(void){
    // insert code to interpret analog to digital interpretation
    // if statements for setting true/false
    return;
}

// get functions
float Buttons::getButtA(void){
    return buttA;
}
float Buttons::getButtA(void){
    return buttB;
}