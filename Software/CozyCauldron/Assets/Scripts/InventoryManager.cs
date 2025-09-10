
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject InventoryDescription;
    public GameObject WorkstationMenu;
    public GameObject CraftingMinigamePanel;
    private bool minigameActive = false;
    private string keySequence = "";
    private int currentKeyIndex = 0;
    public Button workstationButton;
    public string currentWorkstationName;
    public Sprite currentWorkstationSprite;
    private bool menuActivated;
    public bool workstationActivated;
    private bool isWorkstationMenuActive;
    public Sprite trashSprite;
    public Sprite combinedSprite;

    public Sprite waterBreathingPotionSprite;

    public Transform keySequenceContainer; // assign KeySequenceContainer in Inspector
    public GameObject keyImagePrefab;      // a prefab with just an Image component
    public Sprite keyZSprite;
    public Sprite keyXSprite;
    public Sprite keyCSprite;
    public Sprite keyVSprite;

    private Dictionary<string, Sprite> keySprites;
    private List<Image> activeKeyImages = new List<Image>();
    private List<Image> overlayImages = new List<Image>();

    [SerializeField] private ItemSlot[] itemSlots; // Array of inventory item slots
    [SerializeField] private ItemSlot[] workstationSlots; // Array of workstation item slots
    public Image WorkstationImage;

    private int selectedItemIndex = 0; // Track the selected item
    private int selectedWorkstationIndex = 0; // Track the selected workstation item
    private const int columns = 5; // Number of columns in the inventory grid
    private const int rows = 4; // Number of rows in the inventory grid

    private float delayTimer = 0f;
    private bool isDelaying = false;
    private bool justOpened = false; // Track if the workstation menu was just opened

    private List<(Dictionary<string,int> recipe, string resultName, Sprite resultSprite, string resultDesc)> craftingRecipes = new List<(Dictionary<string,int>, string, Sprite, string)>();

    public static InventoryManager Instance;

    private void Awake()
    {
        // Make this a singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

            // Example: Cube + Sphere combination
        craftingRecipes.Add((
            new Dictionary<string,int>
            {
                {"Cube", 1},
                {"Sphere", 1}
            },
            "Combined",
            combinedSprite,
            "Yippee, you made something!"
        ));

        // Water Breathing Potion
        craftingRecipes.Add((
            new Dictionary<string,int>
            {
                {"Starfish", 1},
                {"Clownfish", 1},
                {"Lily Pad", 1},
                {"Barnacle", 1}
            },
            "Water Breathing Potion",
            waterBreathingPotionSprite, 
            "One step closer to becoming a mermaid!"
        ));
    }

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

        keySprites = new Dictionary<string, Sprite>
        {
            { "Z", keyZSprite },
            { "X", keyXSprite },
            { "C", keyCSprite },
            { "V", keyVSprite }
        };

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

        // Run minigame update first if active
        if (minigameActive)
        {
            UpdateCraftingMinigame();
            return; // skip inventory/workstation logic while minigame is running
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
        else if (workstationActivated && !justOpened)
        {
            // If the workstation menu is active, handle navigation for it
            // Stop time when the workstation menu is active
            justOpened = true;

            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            WorkstationMenu.SetActive(true);
            WorkstationImage.sprite = currentWorkstationSprite; 
            InventoryDescription.SetActive(false);
        }
        else if (workstationActivated && justOpened)
        {
            // If the workstation menu is active, handle navigation for it
            // Stop time when the workstation menu is active

            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            WorkstationMenu.SetActive(true);
            WorkstationImage.sprite = currentWorkstationSprite;
            InventoryDescription.SetActive(false);
            HandleNavigation();

            if (Input.GetButtonDown("Interact"))
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
            justOpened = false;
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
                // Move to item slot 9
                selectedItemIndex = 9;
                itemSlots[selectedItemIndex].SetHighlight(true);
                isWorkstationMenuActive = false;
                return;
            }
            if (direction == -1 && selectedWorkstationIndex == 2)
            {
                // Move to item slot 14
                selectedItemIndex = 14;
                itemSlots[selectedItemIndex].SetHighlight(true);
                isWorkstationMenuActive = false;
                return;
            }
            else if (direction == -1 && selectedWorkstationIndex == 4)
            {
                // Move to item slot 19
                selectedItemIndex = 19;
                itemSlots[selectedItemIndex].SetHighlight(true);
                isWorkstationMenuActive = false;
                return;
            }
            else if (direction == 1 && selectedWorkstationIndex == 0)
            {
                // Move to the top right slot from the top left slot
                newIndex = 1;
            }
            else if (direction == columns && selectedWorkstationIndex == 0)
            {
                // Move to the bottom left slot from the top left slot
                newIndex = 2;
            }
            else if (direction == -1 && selectedWorkstationIndex == 1)
            {
                // Move to the top left slot from the top right slot
                newIndex = 0;
            }
             else if (direction == columns && selectedWorkstationIndex == 1)
            {
                // Move to the bottom right slot from the top right slot
                newIndex = 3;
            }
            else if (direction == columns && (selectedWorkstationIndex == 2 || selectedWorkstationIndex == 3))
            {
                // Move to the button when navigating down from slots 2 or 3
                newIndex = 4;
            }
            else if (direction == 1 && selectedWorkstationIndex == 2)
            {
                // Move to the bottom right slot from the bottom left slot
                newIndex = 3;
            }
            else if (direction == -columns && selectedWorkstationIndex == 2)
            {
                // Move to the top left slot from the bottom left slot
                newIndex = 0;
            }
            else if (direction == -1 && selectedWorkstationIndex == 3)
            {
                // Move to the bottom left slot from the bottom right slot
                newIndex = 2;
            }
            else if (direction == -columns && selectedWorkstationIndex == 3)
            {
                // Move to the top right slot from the bottom right slot
                newIndex = 1;
            }
            else if (direction == -columns && selectedWorkstationIndex == 4)
            {
                // Move to the bottom left slot from the button
                newIndex = 2;
            }
            

            // Update the selected index
            selectedWorkstationIndex = newIndex;
            //Debug.Log($"Updated selectedWorkstationIndex: {selectedWorkstationIndex}");


            // Highlight the new slot or button
            if (selectedWorkstationIndex < 4)
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
                    selectedWorkstationIndex = 4;
                    isWorkstationMenuActive = true;
                    return;
                }
                else if (workstationActivated && selectedItemIndex == 14)
                {
                    // Select the bottom left workstation slot
                    workstationSlots[2].SetHighlight(true);
                    selectedWorkstationIndex = 2;
                    isWorkstationMenuActive = true;
                    return;
                }
                else if (workstationActivated && selectedItemIndex == 9)
                {
                    // Select the top right workstation slot
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
        if (isWorkstationMenuActive && selectedWorkstationIndex == 4) // Submit button logic
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

    private void StartCraftingMinigame()
    {
        int sequenceLength = 3;
        List<string> possibleKeys = new List<string> { "Z", "X", "C", "V" };

        // Shuffle the list
        for (int i = 0; i < possibleKeys.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, possibleKeys.Count);
            string temp = possibleKeys[i];
            possibleKeys[i] = possibleKeys[randIndex];
            possibleKeys[randIndex] = temp;
        }

        // Pick the first three keys
        keySequence = string.Join(" ", possibleKeys.GetRange(0, sequenceLength));

        currentKeyIndex = 0;
        minigameActive = true;

        // Clear old icons and overlays
        foreach (Transform child in keySequenceContainer)
            Destroy(child.gameObject);

        activeKeyImages.Clear();
        overlayImages.Clear();

        // Create icons and overlays
        string[] keys = keySequence.Split(' ');
        foreach (string key in keys)
        {
            GameObject keyGO = Instantiate(keyImagePrefab, keySequenceContainer);
            Image keyImg = keyGO.GetComponent<Image>();
            keyImg.sprite = keySprites[key];
            keyImg.color = Color.white;
            activeKeyImages.Add(keyImg);

            // Overlay
            GameObject overlayGO = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            overlayGO.transform.SetParent(keyGO.transform, false);
            Image overlay = overlayGO.GetComponent<Image>();
            overlay.color = new Color(0, 1, 0, 0.5f);
            overlay.gameObject.SetActive(false);
            overlayImages.Add(overlay);

            // Set overlay size
            RectTransform rt = overlayGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 400);
            rt.localPosition = Vector3.zero;
        }

        CraftingMinigamePanel.SetActive(true);
    }

    private void UpdateCraftingMinigame()
    {
        if (!minigameActive) return;

        string[] keys = keySequence.Split(' ');

        if (Input.anyKeyDown)
        {
            string expectedKey = keys[currentKeyIndex].ToLower();

            if (Input.GetKeyDown(expectedKey))
            {
                // Correct key → show overlay green
                overlayImages[currentKeyIndex].gameObject.SetActive(true);

                currentKeyIndex++;

                if (currentKeyIndex >= keys.Length)
                {
                    FinishCraftingMinigame();
                }
            }
            else
            {
                // Wrong key → reset overlays
                foreach (Image overlay in overlayImages)
                    overlay.gameObject.SetActive(false);

                currentKeyIndex = 0;
            }
        }
    }

    private void FinishCraftingMinigame()
    {
        minigameActive = false;
        CraftingMinigamePanel.SetActive(false);

        // Highlight button and unhighlight the workstation slots
        for(int i = 0; i < 4; i++)
        {
            workstationSlots[i].SetHighlight(false);
        }
            
        workstationButton.SetHighlight(true); 
           
        Debug.Log("Minigame complete!");
    }

    private bool MatchesRecipe(Dictionary<string,int> recipe, Dictionary<string,int> currentCounts)
{
    foreach (var kv in recipe)
    {
        if (!currentCounts.TryGetValue(kv.Key, out int count) || count != kv.Value)
            return false; // missing item or wrong quantity
    }
    return true;
}

    private void PerformCombinationAction()
    {
        // Count items in workstation slots
    Dictionary<string,int> itemCounts = new Dictionary<string,int>();
    foreach (ItemSlot slot in workstationSlots)
    {
        if (slot.quantity > 0)
        {
            if (!itemCounts.ContainsKey(slot.itemName))
                itemCounts[slot.itemName] = 0;

            itemCounts[slot.itemName] += slot.quantity;
        }
    }

    // If no items, exit
    if (itemCounts.Count == 0) return;

    bool matched = false;
    foreach (var recipe in craftingRecipes)
    {
        if (MatchesRecipe(recipe.recipe, itemCounts))
        {
            AddItem(recipe.resultName, 1, recipe.resultSprite, recipe.resultDesc);
            matched = true;
            break; // stop after the first match
        }
    }

    if (!matched)
    {
        // If no valid recipe, create trash
        AddItem("Trash", 1, trashSprite, "Whoops, you made trash!");
    }

    // Clear workstation slots
    foreach (ItemSlot slot in workstationSlots)
    {
        while (slot.quantity > 0)
            slot.RemoveItem();
    }

    // Start the minigame
    StartCraftingMinigame();
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


