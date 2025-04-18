# Protorype

## Completed Work
### Hardware
Prototypes for external interface modules such as the buttons, joystick, and IMU classes have been set up with dummy functions to be fleshed out post-PreAlpha. All classes contain private data elements corresponding to the type of data that their individual components output, and are set up so that they can send their info to the rest of the project for data processing and handling.

### ML
Basic gesture database is implemented, which has only been populated with “dummy” data. This data serves as a placeholder for the gesture data until we collect testing data with the controller when we begin programming the gestures we want. The gesture recognition logic has also been integrated, which compares the user-parsed data with the database of pre-coded gestures. Connecting this logic with TensorFlow is the next planned step. Lastly, user-parsed data passes through a basic filter that checks if the data “is real”. The filter eliminates data sets that are composed of strictly 0x00 or strictly 0xff, which indicates the data was collected incorrectly.

### Software
Unity rendering pipeline setup as well as a scene using temporary assets. Player movement and animations are working within the game scene. Interactable items have been created and can be picked up. The inventory system can be opened and the selected inventory slot will populate the item information on the right side. Items of the same type can stack in inventory slots. Items can be passed between the workstation menu and the inventory. Basic crafting logic has been implemented where items are consumed and the crafted item appears in the inventory. 

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
### Not Fixed
- Amount of time before animation starts is inconsistent between actions
- Some animation actions are currently allowed to overlap unintentionally
- Crafted item does not have a separate UI pop up, it currently just appears in the inventory.
