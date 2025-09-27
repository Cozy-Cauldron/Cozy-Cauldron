# Alpha Build
## The Cozy Cauldron
The Cozy Cauldron is a collaborative senior project created for the University of Florida's Computer Engineering Design course. As an overview, this project is a videogame that's being developed alongside a custom hardware controller and machine learning model to create a unique physical and tacticile gameplay experience.

As a game, The Cozy Cauldron is based around the themes of magic and comfort where you play a witch/wizard who gathers ingredients, craft unique potions, and fulfill requests for them to them to the world beyond! By using your trusty magic wand controller, you'll be able to cast spells through specific gestures to interact with the world. With your spells, you can perform great wonders from fishing straight from a lake to simply stirring your cauldron pot.

As an engineering project, The Cozy Cauldron supports its fun gameplay experience through the custom-developed wand-shaped controller, designed to help immerse the player in casting spells. The controller itself is composed of a Raspberry Pi Pico 1 WH board with a RP2040 chip, a MPU6050 IMU sensor module for motion data, an I2C joystick, several buttons componets, and a USB cord that connects to the user's PC, all of which is encased in a 3D-printed wand casing to give the player the true experience of holding magic in their hands. By pairing with our developed machine learning model, ths custom controller is able to identify specific gestures recorded by the MPU6050's motion data and output gesture-specific signals to the game to be interpreted as inputs for in-game interactions.

This project was developed from Spring 2025 to Fall 2025 by:
- Kylie Lennon
- Emilie Doan
- M Vargo
- Ryan Weisse
- Phillip Somchanhmavong

## Completed Work
### Hardware
Code for external interface modules such as the buttons, joystick, and IMU classes has been written so it can be combined with the code that handles IMU communication. Buttons have been physically connected to the microcontroller and can take input. The microcontroller is able to read raw data from the IMU (gyroscope and accelerometer) and transmit it serially over USB connection. The output can currently be displayed in a terminal on a PC. A python script (under ML) converts the serial data to a CSV file for the ML model to read from. 

### ML
Basic gesture database is implemented, which has only been populated with “dummy” data. This data serves as a placeholder for the gesture data until we collect testing data with the controller when we begin programming the gestures we want. The gesture recognition logic has also been integrated, which compares the user-parsed data with the database of pre-coded gestures. Lastly, user-parsed data passes through a basic filter that checks if the data “is real”. The filter eliminates data sets that are composed of strictly 0x00 or strictly 0xff, which indicates the data was collected incorrectly. There is a developed working ML model that loads in training and testing data, and decides which "action" from the training data matches closest with testing data. The model uses random forest classification for comparison and uses PCA and radar plot for data visuals.

### Software
Player movement and animations are working within the game scene for a new in-progress character model. The ability to smoothly transition between scenes has been added for the use of multiple in-game environments in the future. Interactable items have been created and can be picked up. The inventory system can be opened and the selected inventory slot will populate the item information on the right side. Items of the same type can stack in inventory slots. Items can be passed between the workstation menu and the inventory. Basic crafting logic has been implemented, where items are consumed and the crafted item appears in the inventory. Crafting minigame logic and UI has been completed with placeholder keystrokes. Models for environments, witch house interior/exterior, furniture, and ingredients have been created with associated sprites. More player animations have been added for different in-game actions.

## Project Architecture
Beginning with the external interfaces from the controller wand, data is recorded and collected from each of the individual external modules such as the joystick, the buttons, and the motion data from the IMU. After this data is collected, it’s then sent to a persistent state where some data is received and interpreted by the project’s internal ML models to interpret (in)valid gestures. Finally, that data is sent into the game software via keystrokes to be further interpreted into actionable game inputs to which the game responds in turn to.

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
- GPIO (button presses) on controller not recognized when run alongside IMU code
  - Corrected order of GPIO initialization to happen before any IMU data is sent
- confusion matrix error: I believe it could be due to the dummy data being too similar to training data.
- some difficulty with handling labels, I may resort to changing the csv file format to include a gesture id so I can more easily debug this issue
-When crafting the item in the first slot automatically goes to the workstation slot
  - Added justOpened bool to only register E for opening the workstation and not for moving over the item.
- Minigame needs an additional keystroke to populate the sprites
  - Reordering where the sprites are loaded allowed them to populate as the menu opens
- Character and model materials are sometimes see through
  - Recalculate the normals on the body and triangulate faces with more than 3 sides.
- When loading to a previously unloaded scene, inventory is not saved
  - Made inventory persistent by passing the inventoryCanvas between scenes when they are loaded.
- Character gets stuck on the ground sometimes
  - Updated character motion settings to better simulate physics.

### Not Fixed
- Amount of time before animation starts is inconsistent between actions
- Player is able to climb over invisible collider for catching fish
- If the workstation is closed while items are in the crafting slots the items get deleted instead of returned to the inventory








