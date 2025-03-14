#include "Joystick.h"

// default constructor
Joystick::Joystick(void) {
    xAxis = 0;
    yAxis = 0;
}

// init/enable joystick
bool Joystick::init(void){
    // provide power
    // pull pins to enable

    return true;
}

// reset component
bool Joystick::reset(void){
    // kill power
    // pull pins to disable

    xAxis = 0;
    yAxis = 0;

    init();
    return true;
}

// update data elements
void Joystick::update(void){
    /*
    xAxis = (GPIO value / 2) - 512
    yAxis = (GPIO value / 2) - 512
    */
   return;
}

// get functions
float Joystick::getAxisX(void){
    return xAxis;
}
float Joystick::getAxisY(void){
    return yAxis;
}