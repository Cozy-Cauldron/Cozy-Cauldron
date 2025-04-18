
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject InventoryDescription;
    public GameObject WorkstationMenu;
    public Button workstationButton;
    public string currentWorkstationName;
    public Sprite currentWorkstationSprite;
    private bool menuActivated;
    public bool workstationActivated;
    private bool isWorkstationMenuActive;
    public Sprite trashSprite;
    public Sprite combinedSprite;

    [SerializeField] private ItemSlot[] itemSlots; // Array of inventory item slots
    [SerializeField] private ItemSlot[] workstationSlots; // Array of workstation item slots
    public Image WorkstationImage;

    private int selectedItemIndex = 0; // Track the selected item
    private int selectedWorkstationIndex = 0; // Track the selected workstation item
    private const int columns = 5; // Number of columns in the inventory grid
    private const int rows = 4; // Number of rows in the inventory grid

    private float delayTimer = 0f;
    private bool isDelaying = false;

    void Start()
    {
        if (itemSlots == null || itemSlots.Length == 0)
        {
            Debug.LogError("InventoryManager: itemSlots array is null or empty! Assign it in the Inspector.");
            return;
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                Debug.LogError($"InventoryManager: itemSlots[{i}] is null! Ensure all slots are assigned.");
            }
        }

        // Ensure only the first item is highlighted at start
        if (itemSlots.Length > 0)
        {
            itemSlots[selectedItemIndex].SetHighlight(true);
        }

    }

    void Update()
    {
        if (isDelaying)
        {
            // Increment the timer
            delayTimer += Time.deltaTime;

            // Check if the delay has passed
            if (delayTimer >= 0.5f) // 0.5 seconds delay
            {
                Time.timeScale = 1; // Reset time scale
                isDelaying = false; // Stop the delay
                delayTimer = 0f; // Reset the timer
            }
            return; // Skip the rest of Update while delaying
        }

        if (Input.GetButtonDown("Inventory"))
        {
            if(workstationActivated)
            {
                // If the workstation menu is active, close it
                workstationActivated = false;
                InventoryMenu.SetActive(false);
                WorkstationMenu.SetActive(false);
                InventoryDescription.SetActive(false);
                Time.timeScale = 1; // Reset time scale
                return; // Exit early
            }
            else if(menuActivated)
            {
                // If the inventory menu is open, close it
                menuActivated = false;
                InventoryMenu.SetActive(false);
                WorkstationMenu.SetActive(false);
                InventoryDescription.SetActive(false);
                Time.timeScale = 1; // Reset time scale
                return; // Exit early
            }
            else 
            {
                //open inventory
                menuActivated = true;
                InventoryMenu.SetActive(true);
                WorkstationMenu.SetActive(false);
                InventoryDescription.SetActive(true);
            }
            
            Time.timeScale = menuActivated ? 0 : 1;

            if (menuActivated)
            {
                // When opening, immediately update the UI so description and name show up
                itemSlots[selectedItemIndex].SetHighlight(true);
            }
        }

        if (menuActivated) // Only allow navigation if inventory is open
        {
            WorkstationMenu.SetActive(false);
            InventoryDescription.SetActive(true);
            HandleNavigation();
            if (Input.GetButtonDown("Remove") && itemSlots[selectedItemIndex].quantity > 0)
            {
                itemSlots[selectedItemIndex].RemoveItem();
            }
        }
        else if (workstationActivated)
        {
            // If the workstation menu is active, handle navigation for it
            // Stop time when the workstation menu is active
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            WorkstationMenu.SetActive(true);
            WorkstationImage.sprite = currentWorkstationSprite; 
            InventoryDescription.SetActive(false);
            HandleNavigation();

            if(Input.GetButtonDown("Interact"))
            {
                MoveItemBetweenSlots();
                return;
            }
        }
        else
        {
            InventoryMenu.SetActive(false);
            WorkstationMenu.SetActive(false);
            InventoryDescription.SetActive(false);
            menuActivated = false;
            workstationActivated = false;
            // Reset time scale to 1 when neither menu is active
            Time.timeScale = 1;
        }
    }

    private void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(columns);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-columns);
        }
    }

    private void MoveSelection(int direction)
    {
        // Unhighlight all workstation slots
        foreach (ItemSlot slot in workstationSlots)
        {
            slot.SetHighlight(false);
        }

        // Unhighlight the workstation button
        workstationButton.SetHighlight(false);

        // Unhighlight the current inventory slot
        itemSlots[selectedItemIndex].SetHighlight(false);

        if (isWorkstationMenuActive)
        {
            int newIndex = selectedWorkstationIndex;
            if (direction == -1 && selectedWorkstationIndex == 0)
            {
                // Move to item slot 14
                selectedItemIndex = 14;
                itemSlots[selectedItemIndex].SetHighlight(true);
                isWorkstationMenuActive = false;
                return;
            }
            else if (direction == -1 && selectedWorkstationIndex == 3)
            {
                // Move to item slot 19
                selectedItemIndex = 19;
                itemSlots[selectedItemIndex].SetHighlight(true);
                isWorkstationMenuActive = false;
                return;
            }
            else if (direction == 1 && selectedWorkstationIndex == 0)
            {
                // Move to the middle slot from the left slot
                newIndex = 1;
            }
            else if (direction == 1 && selectedWorkstationIndex == 1)
            {
                // Move to the right slot from the middle slot
                newIndex = 2;
            }
            else if (direction == -1 && selectedWorkstationIndex == 1)
            {
                // Move to the left from the middle slot
                newIndex = 0;
            }
            else if (direction == -1 && selectedWorkstationIndex == 2)
            {
                // Move to the middle from the right slot
                newIndex = 1;
            }
            else if (direction == columns && selectedWorkstationIndex < 3)
            {
                // Move to the button when navigating down from the first row
                newIndex = 3;
            }
            else if (direction == -columns && selectedWorkstationIndex == 3)
            {
                // Move to the center slot when navigating up from the button
                newIndex = 1;
            }

            // Update the selected index
            selectedWorkstationIndex = newIndex;
            //Debug.Log($"Updated selectedWorkstationIndex: {selectedWorkstationIndex}");


            // Highlight the new slot or button
            if (selectedWorkstationIndex < 3)
            {
                workstationSlots[selectedWorkstationIndex].SetHighlight(true);
            }
            else
            {
                // Highlight the button
                workstationButton.SetHighlight(true); 
            }
        }
        else
        {
            // Handle navigation logic for inventory slots
            int newIndex = selectedItemIndex + direction;

            // Wrap-around logic
            if (direction == 1 && (selectedItemIndex + 1) % columns == 0)
            {
                if (workstationActivated && selectedItemIndex == 19)
                {
                    // Select the button
                    workstationButton.SetHighlight(true);
                    selectedWorkstationIndex = 3;
                    isWorkstationMenuActive = true;
                    return;
                }
                else if (workstationActivated)
                {
                    // Select the first workstation slot
                    workstationSlots[0].SetHighlight(true);
                    selectedWorkstationIndex = 0;
                    isWorkstationMenuActive = true;
                    return;
                }
                else
                {
                    // Stay in place if moving right at the end of a row
                    newIndex = selectedItemIndex;
                }
            }
            else if (direction == -1 && selectedItemIndex % columns == 0)
            {
                // Stay in place if moving left at the start of a row
                newIndex = selectedItemIndex;
            }
            else if (newIndex >= itemSlots.Length)
            {
                // Prevent moving down past the last row
                newIndex = selectedItemIndex;
            }
            else if (newIndex < 0)
            {
                // Prevent moving up past the first row
                newIndex = selectedItemIndex;
            }

            selectedItemIndex = newIndex;

            // Highlight the new slot
            itemSlots[selectedItemIndex].SetHighlight(true);
        }
    }


    private void MoveItemBetweenSlots()
    {
        if (isWorkstationMenuActive && selectedWorkstationIndex == 3) // Submit button logic
        {
            //Debug.Log("Combination submitted!");
            PerformCombinationAction();
            return; // Exit after handling the submit button
        }

        if (!isWorkstationMenuActive) // Moving from inventory to workstation
        {
            ItemSlot inventorySlot = itemSlots[selectedItemIndex];

            if (inventorySlot.quantity > 0)
            {
                // Check if there is a matching item in the workstation slots
                foreach (ItemSlot workstationSlot in workstationSlots)
                {
                    if (workstationSlot.itemName == inventorySlot.itemName && workstationSlot.quantity > 0)
                    {
                        // Add one item to the matching workstation slot
                        int leftover = workstationSlot.AddItem(
                            inventorySlot.itemName,
                            1, // Move only one item
                            inventorySlot.itemSprite,
                            inventorySlot.itemDescription
                        );

                        // Decrement quantity in the inventory slot
                        if (leftover == 0)
                        {
                            inventorySlot.RemoveItem();
                        }
                        return; // Exit after moving the item
                    }
                }

                // If no matching slot is found, move to the first empty workstation slot
                foreach (ItemSlot workstationSlot in workstationSlots)
                {
                    if (workstationSlot.quantity == 0)
                    {
                        int leftover = workstationSlot.AddItem(
                            inventorySlot.itemName,
                            1, // Move only one item
                            inventorySlot.itemSprite,
                            inventorySlot.itemDescription
                        );

                        if (leftover == 0)
                        {
                            inventorySlot.RemoveItem();
                        }
                        return; // Exit after moving the item
                    }
                }
            }
        }
        else // Moving from workstation to inventory
        {
            ItemSlot workstationSlot = workstationSlots[selectedWorkstationIndex];

            if (workstationSlot.quantity > 0)
            {
                // Check if there is a matching item in the inventory slots
                foreach (ItemSlot inventorySlot in itemSlots)
                {
                    if (inventorySlot.itemName == workstationSlot.itemName && inventorySlot.quantity > 0)
                    {
                        // Add one item to the matching inventory slot
                        int leftover = inventorySlot.AddItem(
                            workstationSlot.itemName,
                            1, // Move only one item
                            workstationSlot.itemSprite,
                            workstationSlot.itemDescription
                        );

                        // Decrement quantity in the workstation slot
                        if (leftover == 0)
                        {
                            workstationSlot.RemoveItem();
                        }
                        return; // Exit after moving the item
                    }
                }

                // If no matching slot is found, move to the first empty inventory slot
                foreach (ItemSlot inventorySlot in itemSlots)
                {
                    if (inventorySlot.quantity == 0)
                    {
                        int leftover = inventorySlot.AddItem(
                            workstationSlot.itemName,
                            1, // Move only one item
                            workstationSlot.itemSprite,
                            workstationSlot.itemDescription
                        );

                        if (leftover == 0)
                        {
                            workstationSlot.RemoveItem();
                        }
                        return; // Exit after moving the item
                    }
                }
            }
        }
    }

    private void PerformCombinationAction()
    {
        int total = 0;
        int Cube = 0;
        int Sphere = 0;


        foreach (ItemSlot slot in workstationSlots)
        {
            if(slot.itemName == "Trash") break;
            if (slot.itemName == "Cube") Cube += slot.quantity;
            if (slot.itemName == "Sphere") Sphere += slot.quantity;
            total += slot.quantity;
        }
        if(total == 0)
        {
            // No items in the workstation slots
            return;
        }
        // Check if the combination is valid
        if (Cube == 2 && Sphere == 1)
        {
            //invalid combintation
            //make combined iterm
            int temp = AddItem("Combined", 1, combinedSprite, "Yippee, you made something!");

        }
        else
        {
            //invalid combintation
            //make trash
            int temp = AddItem("Trash", 1, trashSprite, "Whoops, you made trash! Make sure you throw it out!");
        }
       
        //Clear the workstation slots
        foreach (ItemSlot slot in workstationSlots)
        {
            while (slot.quantity > 0)
            {
                slot.RemoveItem(); // Remove one item from the workstation slot
            }
        }
    }


    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].isFull == false && (itemSlots[i].itemName == itemName || itemSlots[i].quantity==0))
            {
                int leftOverItems = itemSlots[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if(leftOverItems > 0)
                {
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                }   
                return leftOverItems; 
            }
        }
        return quantity;
    }

}


