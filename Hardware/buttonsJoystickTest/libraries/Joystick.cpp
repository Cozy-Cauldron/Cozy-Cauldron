#include "Joystick.h"

#define GND_Pin 1
#define PWR_Pin 2

// GPIO ADC pins are GPIO[26:29], which are [38:41]
#define xAxisPin 38
#define yAxisPin 39
// For select_input, they correspond to [0:3] from GPIO[26:29]
#define xPinSelect 0
#define yPinSelect 1

// Placeholder GPIO Pin 
#define zButtonPin 37

typedef enum gpio_function_rp2040 gpio_function_t;

// default constructor
Joystick::Joystick(void) {
    xAxis = 0;
    yAxis = 0;
    zButtonPressed = false;
}

// init/enable joystick
bool Joystick::init(void){
    // Provide Power? (make sure it's 3.3V!)

        // Set up ADC Axis GPIOs
    // potentially need to activate clk_adc
        // check to make sure adc_init() activates clk
    // Remember it's independent 48MHz clock
    // Initialize ADCs & set GPIO pins to ADC-mode
    adc_init();
    adc_gpio_init(xAxisPin);
    adc_gpio_init(yAxisPin);

    // Potentially setup ADC Interrupts?
    // adc_irq_set_enabled(true);

        // Set up ZButton GPIO
    // GPIO Init
    gpio_init(zButtonPin);
    // EN GPIO Input
    gpio_set_input_enabled(zButtonPin, true);

    // Look into this to define function more clearly?
        // Either PIO or SIO?
    // gpio_set_function(zButtonPin, GPIO_FUNC_PIO);

    // Pull up
    gpio_pull_up(zButtonPin);

    return true;
}

// reset component
bool Joystick::reset(void){
    // Disable zButton GPIO
    gpio_deinit(zButtonPin);

    // Enter code to disable ADC (Clear CS.EN?)

    // Drain ADC FIFO buffers
    adc_fifo_drain();

    xAxis = 0;
    yAxis = 0;
    zButtonPressed = false;

    init();
    return true;
}

// update data elements
void Joystick::update(void){
    // Note: ADC has 12-bits
    // 0~4095 sent from ADC
    // -2048~2047 calculated for elements

    // Left=Negative, Right=Positive
    adc_select_input(xPinSelect);   
    xAxis = (adc_read() / 2) - 2048;
    // Up=Negative, Right=Positive
    adc_select_input(yPinSelect);
    yAxis = (adc_read() / 2) - 2048;

    // Add code later to debounce?
    // Joystick low(0) when pressed.
    zButtonPressed = !(gpio_get(zButtonPin));

    return;
}

// get functions
float Joystick::getAxisX(void){
    return xAxis;
}
float Joystick::getAxisY(void){
    return yAxis;
}
bool Joystick::getZButton(void){
    return zButtonPressed;
}