# Protorype

## Completed Work
### Hardware
Code for external interface modules such as the buttons, joystick, and IMU classes has been written so it can be combined with the code that handles IMU communication. Buttons have been physically connected to the microcontroller and can take input. The microcontroller is able to read raw data from the IMU (gyroscope and accelerometer) and transmit it serially over USB connection. The output can currently be displayed in a terminal on a PC. Code has been written to convert the serial data to a CSV file for the ML model to read from. 

### ML
Basic gesture database is implemented, which has only been populated with “dummy” data. This data serves as a placeholder for the gesture data until we collect testing data with the controller when we begin programming the gestures we want. The gesture recognition logic has also been integrated, which compares the user-parsed data with the database of pre-coded gestures. Connecting this logic with TensorFlow is the next planned step. Lastly, user-parsed data passes through a basic filter that checks if the data “is real”. The filter eliminates data sets that are composed of strictly 0x00 or strictly 0xff, which indicates the data was collected incorrectly.

### Software
Player movement and animations are working within the game scene for a new in-progress character model. The ability to smoothly transition between scenes has been added (to a limited functionality) for the use of multiple in-game environments in the future. Interactable items have been created and can be picked up. The inventory system can be opened and the selected inventory slot will populate the item information on the right side. Items of the same type can stack in inventory slots. Items can be passed between the workstation menu and the inventory. Basic crafting logic has been implemented where items are consumed and the crafted item appears in the inventory. 

## Project Architecture
Beginning with the external interfaces from the controller wand, data is recorded and collected from each of the individual external modules such as the joystick, the buttons, and the motion data from the IMU. After this data is collected, it’s then sent to a persistent state where some data is received and interpreted by the project’s internal ML models to interpret (in)valid gestures. Finally, that data is sent into the game software to be further interpreted into actionable game inputs to which the game responds in turn to.

Data collected via hardware -> interpretation algorithm -> output interface -> game inputs

## Known Bugs
### Fixed
- Item image sprite removed slot background. 
  - Added a parent UI panel component to the sprite so it would always have a background.
- Character can still move when the inventory is open. 
  - Changed timescale to 0 in the inventory manager class when the inventory opens and back to 1 when it closes.
- Distance object moves is significantly smaller than intended 
  - Used a different (more compatible) way of moving object instead of root movement
- PC not recognizing connected Pico board.
  - Replaced USB cord (was previously charge-only)
- CMake fails for Pico C files.
  - Hard-coded PICO_BOARD to pico_w in CMakeLists.txt
- Items of the same type do not stack in the inventory.
  - Checked if item of the same name already exist and add to existing quantity.
- Some animation actions are currently allowed to overlap unintentionally
  - When transitioning to new main character model/prefab, this problem naturally went away when re-engineering and designing animations
- Serial output not working while IMU is being read.
  - Rebuilt project, corrected dependencies (TinyUSB, picotool LibUSB)

### Not Fixed
- Amount of time before animation starts is inconsistent between actions 
- Crafted item does not have a separate UI pop up, it currently just appears in the inventory.
- Character materials allow to see through their body sometimes
- Scene completley resets on load
  - When loading to a previously unloaded scene, any changes are not saved (inventory or environment)
- Character gets stuck on the ground sometimes
- GPIO (button presses) on controller not recognized when run alongside IMU code
