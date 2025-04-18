#include <stdio.h>
#include <string>
#include <cstring>

#include "pico/stdlib.h"
#include "hardware/gpio.h"
#include "hardware/adc.h"

#include "libraries/Buttons.h"
#include "libraries/Joystick.h"

int main()
{
    Joystick joyTest;
    Buttons butTest;

    std::string xAxis = "";
    std::string yAxis = "";
    std::string zBut = "";

    std::string aBut = "";
    std::string bBut = "";

    std::string result = "";
    
    stdio_init_all();
    joyTest.init();
    butTest.init();

    while (true) {
        joyTest.update();
        if (joyTest.getAxisX() < 0){
            xAxis = "Left";
        } else{
            xAxis = "Right";
        }
        if (joyTest.getAxisY() < 0){
            yAxis = "Up";
        } else{
            yAxis = "Down"; 
        }
        if (joyTest.getZButton()){
            zBut = "Z-Button Pressed";
        } else {
            zBut = "Z-Button Not Pressed";
        }

        result = "Joystick: X = " + std::to_string(joyTest.getAxisX()) + "(" + xAxis + "), " 
            + "Y = " + std::to_string(joyTest.getAxisY()) + "(" + yAxis + "), "
            + zBut + "\n";
        printf(result.c_str());

        butTest.update();
        if (butTest.getButtA()){
            aBut = "A-Button Pressed";
        } else {
            aBut = "A-Button Not Pressed";
        }
        if(butTest.getButtB()){
            bBut = "B-Button Pressed";
        } else {
            bBut = "B-Button Not Pressed";
        }

        result = aBut + " and " + bBut + "\n";
        printf(result.c_str());
        
        sleep_ms(1000);
    }
}
