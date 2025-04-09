using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;

    [SerializeField] private ItemSlot[] itemSlots; // Array of item slots

    private int selectedItemIndex = 0; // Track the selected item
    private const int columns = 5; // Number of columns in the inventory grid
    private const int rows = 4; // Number of rows in the inventory grid

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
        if (Input.GetButtonDown("Inventory"))
        {
            menuActivated = !menuActivated;
            InventoryMenu.SetActive(menuActivated);
            Time.timeScale = menuActivated ? 0 : 1;

            if (menuActivated) 
            {
            // When opening, immediately update the UI so description and name show up
                itemSlots[selectedItemIndex].SetHighlight(true);
            }
        }

        if (menuActivated) // Only allow navigation if inventory is open
        {
            HandleNavigation();
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
        // Unhighlight the current slot
        itemSlots[selectedItemIndex].SetHighlight(false);

        // Calculate new index
        int newIndex = selectedItemIndex + direction;

        // **Wrap Around Logic**
        if (direction == 1 && (selectedItemIndex + 1) % columns == 0) 
        {
            // If moving right and at the end of a row, stay in place
            newIndex = selectedItemIndex;
        }
        else if (direction == -1 && selectedItemIndex % columns == 0) 
        {
            // If moving left and at the start of a row, stay in place
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
