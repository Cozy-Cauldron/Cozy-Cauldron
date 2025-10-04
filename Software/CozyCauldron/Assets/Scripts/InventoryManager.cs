
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject InventoryDescription;
    public GameObject WorkstationMenu;
    public GameObject CraftingMinigamePanel;
    public GameObject TaskPanel;
    public GameObject StartMenu;
    public GameObject SaveMenu;
    public GameObject EndMenu;
    private bool minigameActive = false;
    private string keySequence = "";
    private int currentKeyIndex = 0;
    public Button workstationButton;
    public string currentWorkstationName;
    public Sprite currentWorkstationSprite;
    private bool menuActivated;
    public bool workstationActivated;
    public bool taskPanelActivated;
    private bool isWorkstationMenuActive;

    public Sprite oopsPotionSprite;

    private int currentPageIndex;
    public TaskPage[] pages;
    public Image characterImageUI;
    public TMP_Text characterNameUI;  
    public TMP_Text characterTextUI;
    public Image potionImageUI;
    public TMP_Text potionNameUI;
    public TMP_Text potionTextUI;

    public Sprite waterBreathingPotionSprite;
    public Sprite swimmingPotionSprite;
    public Sprite jumpingPotionSprite;
    public Sprite flyingPotionSprite;
    public Sprite rainbowPotionSprite;
    public Sprite armorPotionSprite;

    public Sprite kylieSprite;
    public Sprite emilieSprite;
    public Sprite mSprite;
    public Sprite phillipSprite;
    public Sprite ryanSprite;
    public Sprite profSprite;

    public Sprite submitSprite;
    public Sprite doneSprite;

    public Transform keySequenceContainer; 
    public GameObject keyImagePrefab;      // a prefab with just an Image component
    public Sprite keyZSprite;
    public Sprite keyXSprite;
    public Sprite keyCSprite;
    public Sprite keyVSprite;
    public Sprite keyBSprite;
    public Sprite keyNSprite;

    private Dictionary<string, Sprite> keySprites;
    private List<Image> activeKeyImages = new List<Image>();
    private List<Image> overlayImages = new List<Image>();

    [SerializeField] private ItemSlot[] itemSlots; // Array of inventory item slots
    [SerializeField] private ItemSlot[] workstationSlots; // Array of workstation item slots
    public Image WorkstationImage;

    public Button[] saveButtons;
    public int selectedSaveButtonIndex = 0; // 0 = Save, 1 = New, 2 = Load, 3 = Close

    public Image saveStatus;
    public Sprite noStatus;
    public Sprite saved;
    public Sprite newSave;
    public Sprite loaded;

    public Button[] taskPanelButtons;
    public int selectedTaskButtonIndex = 0; // 0 = Left, 1 = Submit, 2 = Right

    private int selectedItemIndex = 0; // Track the selected item
    private int selectedWorkstationIndex = 0; // Track the selected workstation item
    private const int columns = 5; // Number of columns in the inventory grid
    private const int rows = 4; // Number of rows in the inventory grid

    private float delayTimer = 0f;
    private bool isDelaying = false;
    private bool justOpened = false; // Track if the workstation was just opened

    public bool startMenu = true;
    public bool saveMenu = false;
    public bool endMenu = false;
    public bool saveMenuJustOpened = false;



    private List<(Dictionary<string, int> recipe, string resultName, Sprite resultSprite, string resultDesc)> craftingRecipes = new List<(Dictionary<string, int>, string, Sprite, string)>();

    private SaveData loadedSaveData; // Store loaded data temporarily

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
        
        pages = new TaskPage[6];

        pages[0] = new TaskPage 
        {
            completed = false,
            characterSprite = kylieSprite,
            characterName = "Kylie",
            characterTextBefore = "I love to swim! The only issue is I have to come up for air. I heard of something called a Water Breathing Potion, can you make me one?",
            characterTextAfter = "Thanks! I'm going to explore the bottom of the ocean until I am a prune!",
            potionSprite = waterBreathingPotionSprite,
            potionName = "Water Breathing Potion",
            potionText = "1 Starfish\r\n2 Barnacles\r\n1 Clownfish\r\n2 Lily Pads"
        };

        pages[1] = new TaskPage 
        {
            completed = false,
            characterSprite = emilieSprite,
            characterName = "Emilie",
            characterTextBefore = "I'm hosting a party and the theme is rainbows! I want to make rainbows using a Rainbow Potion. Can you get one for me?",
            characterTextAfter = "Thank you! I can't wait to be the life of the party :)",
            potionSprite = rainbowPotionSprite,
            potionName = "Rainbow Potion",
            potionText = "2 Sunflowers\r\n2 Roses\r\n2 Daisies\r\n1 Goldfish"
        };

        pages[2] = new TaskPage
        {
            completed = false,
            characterSprite = mSprite,
            characterName = "M",
            characterTextBefore = "I want to become the toughest fighter. My defensive skills aren't the best so I take a lot of hits. Can you make me an Armor Potion?",
            characterTextAfter = "With this I'll definitely be able to take a few punches. My opponent might even break a knuckle!",
            potionSprite = armorPotionSprite,
            potionName = "Armor Potion",
            potionText = "1 Beetle\r\n1 Pufferfish \r\n3 Shells\r\n1 Roly Poly"
        };

        pages[3] = new TaskPage
        {
            completed = false,
            characterSprite = ryanSprite,
            characterName = "Ryan",
            characterTextBefore = "Have you ever noticed that we can't jump? It makes it really hard to play my favorite sports. Can you make me a Jumping Potion?",
            characterTextAfter = "My skills will definitely improve with this! I might even become the MVP",
            potionSprite = jumpingPotionSprite,
            potionName = "Jumping Potion",
            potionText = "1 Frog\r\n3 Mushrooms\r\n1 Bunny\r\n1 Jumping Spider"
        };

        pages[4] = new TaskPage
        {
            completed = false,
            characterSprite = phillipSprite,
            characterName = "Phillip",
            characterTextBefore = "I trying to learn how to swim but unfortunately I'm not very good, I just keep sinking! Can you help me learn with a Swimming Potion?",
            characterTextAfter = "This will really help during my next swim lesson. Hopefully I will float!",
            potionSprite = swimmingPotionSprite,
            potionName = "Swimming Potion",
            potionText = "1 Sturgean\r\n1 Bass\r\n1 Salmon\r\n3 Seaweed"
        };

        pages[5] = new TaskPage
        {
            completed = false,
            characterSprite = profSprite,
            characterName = "Dr.Del Rocco",
            characterTextBefore = "I love watching the bugs and the birds fly through the air. I would love to join them, can you make me a Flying Potion?",
            characterTextAfter = "It's time for takeoff! I hope I can figure out how to land!",
            potionSprite = flyingPotionSprite,
            potionName = "Flying Potion",
            potionText = "1 Lady Bug\r\n1 Butterfly Fish\r\n2 Dandelions\r\n2 Eggs"
        };


        UpdatePageUI(); // show first page

        // Water Breathing Potion
        craftingRecipes.Add((
            new Dictionary<string,int>
            {
                {"Starfish", 1},
                {"Clownfish", 1},
                {"Lily Pad", 2},
                {"Barnacle", 2}
            },
            "Water Breathing Potion",
            waterBreathingPotionSprite, 
            "One step closer to becoming a mermaid!"
        ));

        // Swimming Potion
        craftingRecipes.Add((
            new Dictionary<string, int>
            {
                {"Sturgean", 1},
                {"Bass", 1},
                {"Salmon", 2},
                {"Seaweed", 2}
            },
            "Swimming Potion",
            swimmingPotionSprite,
            "With this I can become one with the fish!"
        ));

        // Rainbow Potion
        craftingRecipes.Add((
            new Dictionary<string, int>
            {
                {"Sunflower", 2},
                {"Rose", 2},
                {"Daisy", 2},
                {"Goldfish", 1}
            },
            "Rainbow Potion",
            rainbowPotionSprite,
            "I wonder if this will help me find a pot of gold?"
        ));

        // Jumping Potion
        craftingRecipes.Add((
            new Dictionary<string, int>
            {
                {"Frog", 1},
                {"Mushroom", 3},
                {"Bunny", 1},
                {"Jumping Spider", 1}
            },
            "Jumping Potion",
            jumpingPotionSprite,
            "Don't give this to a monkey, he might jump on your bed!"
        ));

        // Flying Potion
        craftingRecipes.Add((
            new Dictionary<string, int>
            {
                {"Lady Bug", 1},
                {"Butterfly Fish", 1},
                {"Dandelion", 2},
                {"Egg", 2}
            },
            "Flying Potion",
            flyingPotionSprite,
            "I believe I can fly!"
        ));

        // Armor Potion
        craftingRecipes.Add((
            new Dictionary<string, int>
            {
                {"Beetle", 1},
                {"Pufferfish", 1},
                {"Shell", 3},
                {"Roly Poly", 1}
            },
            "Armor Potion",
            armorPotionSprite,
            "I'll be invincible with this!"
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
            { "V", keyVSprite },
            { "N", keyNSprite },
            { "B", keyBSprite }
        };

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Save with K
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L)) // Load with L
        {
            Load();
        }

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
            else if(taskPanelActivated)
            {
                // If the task panel is open, close it
                taskPanelActivated = false;
                TaskPanel.SetActive(false);
                for (int i = 0; i < pages.Length; i++)
                {
                    if (!pages[i].completed)
                    {
                        Time.timeScale = 1; // Reset time scale
                        return;
                    }
                }
                //open end menu
                endMenu = true;
                EndMenu.SetActive(true);
                return; 
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
        else if (taskPanelActivated && !justOpened)
        {
            //Debug.Log("Opened for first time!");
            justOpened = true;
            Time.timeScale = 0;
            TaskPanel.SetActive(true);

            selectedTaskButtonIndex = 1;
            currentPageIndex = 0;
            // Unhighlight all buttons first
            foreach (Button btn in taskPanelButtons)
                btn.SetHighlight(false);
            taskPanelButtons[selectedTaskButtonIndex].SetHighlight(true);
        }
        else if (taskPanelActivated && justOpened)
        {
            // If the task panel is active, show it
            Time.timeScale = 0;
            TaskPanel.SetActive(true);
            HandleTaskNavigation();
        }
        else if (startMenu)
        {
            Time.timeScale = 0;
            if (Input.GetButtonDown("Interact"))
            {
                //close start menu
                startMenu = false;
                StartMenu.SetActive(false);
                //open save menu
                saveMenu = true;
                SaveMenu.SetActive(true);
                return;
            }
        }
        else if (endMenu)
        {
            Time.timeScale = 0;
            if (Input.GetButtonDown("Interact"))
            {
                //reset save file
                NewSave();
                //close end menu
                endMenu = false;
                EndMenu.SetActive(false);
                //open start menu
                startMenu = true;
                StartMenu.SetActive(true);
                return;
            }
        }
        else if (saveMenu && !justOpened)
        {
            justOpened = true;
            Time.timeScale = 0;
            SaveMenu.SetActive(true);
            selectedSaveButtonIndex = 0;
            saveStatus.sprite = noStatus;
            // Unhighlight all buttons first
            foreach (Button btn in saveButtons)
                btn.SetHighlight(false);
            saveButtons[selectedSaveButtonIndex].SetHighlight(true);
        }
        else if (saveMenu && justOpened)
        {
            Time.timeScale = 0;
            SaveMenu.SetActive(true);
            HandleSaveNavigation();
            // Unhighlight all buttons first
            foreach (Button btn in saveButtons)
                btn.SetHighlight(false);
            saveButtons[selectedSaveButtonIndex].SetHighlight(true);
        }
        else
        {
            InventoryMenu.SetActive(false);
            WorkstationMenu.SetActive(false);
            InventoryDescription.SetActive(false);
            TaskPanel.SetActive(false);
            taskPanelActivated = false;
            menuActivated = false;
            workstationActivated = false;
            saveMenu = false;
            startMenu = false;
            endMenu = false;
            justOpened = false;
            // Reset time scale to 1 when neither menu is active
            Time.timeScale = 1;
        }
    }

    public void NewSave()
    {
        SaveData data = new SaveData();
        //set all tasks to false
        bool[] completedPages = new bool[pages.Length];
        for (int i = 0; i < pages.Length; i++)
        {
            completedPages[i] = false;
        }
        data.completedPages = completedPages;

        //current scene
        data.currentScene = "Inside House";

        //player location
        data.playerX = 27.9f;
        data.playerY = 1.1f;
        data.playerZ = 31.5f;

        //inventory empty

        SaveSystem.SaveGame(data);
    }
    public void Save()
    {
        SaveData data = new SaveData();
        //completed tasks
        bool[] completedPages = new bool[pages.Length];
        for (int i = 0; i < pages.Length; i++)
        {
            completedPages[i] = pages[i].completed;
        }
        data.completedPages = completedPages;

        //current scene
        data.currentScene = SceneManager.GetActiveScene().name;

        //character location
        GameObject player = GameObject.Find("MainCharacter");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
            return;
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the player object!");
            return;
        }
        Vector3 pos = playerMovement.GetPosition();
        data.playerX = pos.x;
        data.playerY = pos.y;
        data.playerZ = pos.z;

        //inventory
        foreach (ItemSlot inventorySlot in itemSlots)
        {
            if (inventorySlot.quantity > 0)
            {
                data.itemNames.Add(inventorySlot.itemName);
                data.itemCounts.Add(inventorySlot.quantity);
                data.itemSprites.Add(inventorySlot.itemSprite);
                data.itemDescriptions.Add(inventorySlot.itemDescription);
            }
        }
        SaveSystem.SaveGame(data);
    }

    public void Load()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("No save data found!");
            return;
        }

        // Save data locally for use after scene is loaded
        loadedSaveData = data;

        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Always reload the saved scene so objects refresh
        SceneManager.LoadScene(data.currentScene, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadedSaveData == null) return;

        // Unsubscribe so we don't run this multiple times
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Start a coroutine to wait one frame before restoring data
        StartCoroutine(LoadAfterScene());
    }

    private System.Collections.IEnumerator LoadAfterScene()
    {
        // Wait until end of frame so all objects in the scene are initialized
        yield return new WaitForEndOfFrame();

        currentPageIndex = 0;

        // Restore completed tasks
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].completed = i < loadedSaveData.completedPages.Length ? loadedSaveData.completedPages[i] : false;
        }
        UpdatePageUI();

        // Restore player position
        GameObject player = GameObject.Find("MainCharacter");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector3 savedPos = new Vector3(loadedSaveData.playerX, loadedSaveData.playerY, loadedSaveData.playerZ);
                playerMovement.SetPosition(savedPos);
            }
            else
            {
                Debug.LogError("PlayerMovement component not found on GREEN!");
            }
        }
        else
        {
            Debug.LogError("Player object 'GREEN' not found in the scene!");
        }

        // Clear inventory slots
        foreach (ItemSlot inventorySlot in itemSlots)
        {
            inventorySlot.SetHighlight(false);
            while (inventorySlot.quantity > 0)
                inventorySlot.RemoveItem();
        }

        // Load saved items into inventory
        for (int i = 0; i < loadedSaveData.itemNames.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].AddItem(
                loadedSaveData.itemNames[i],
                loadedSaveData.itemCounts[i],
                loadedSaveData.itemSprites[i],
                loadedSaveData.itemDescriptions[i]
            );
        }

        // Highlight the first item
        selectedItemIndex = 0;
        if (itemSlots.Length > 0)
            itemSlots[selectedItemIndex].SetHighlight(true);

        // Clear temporary save data reference
        loadedSaveData = null;
    }


    private void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(columns);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-columns);
        }
    }

    private void HandleTaskNavigation()
    {
        // Move left/right
        if (Input.GetKeyDown(KeyCode.D)) // Right
        {
            foreach (Button btn in taskPanelButtons)
                btn.SetHighlight(false);
            selectedTaskButtonIndex = Mathf.Min(selectedTaskButtonIndex + 1, 2);
            taskPanelButtons[selectedTaskButtonIndex].SetHighlight(true);
            //Debug.Log(selectedTaskButtonIndex);
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Left
        {
            foreach (Button btn in taskPanelButtons)
                btn.SetHighlight(false);
            selectedTaskButtonIndex = Mathf.Max(selectedTaskButtonIndex - 1, 0);
            taskPanelButtons[selectedTaskButtonIndex].SetHighlight(true);
            //Debug.Log(selectedTaskButtonIndex);
        }
        else
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (selectedTaskButtonIndex == 0) //left
                {
                    //change page
                    //Debug.Log("Left");
                    currentPageIndex--;
                    if (currentPageIndex < 0)
                    {
                        currentPageIndex = pages.Length - 1;
                    }
                    UpdatePageUI();
                }
                else if (selectedTaskButtonIndex == 1) //submit
                {
                    //check if potion is in inventory
                    foreach (ItemSlot inventorySlot in itemSlots)
                    {
                        if (inventorySlot.itemName == pages[currentPageIndex].potionName && inventorySlot.quantity > 0)
                        {
                            pages[currentPageIndex].completed = true;
                            inventorySlot.RemoveItem();
                            UpdatePageUI();
                            return;
                        }
                    }
                }
                else if (selectedTaskButtonIndex == 2) //right
                {
                    //change page
                    //Debug.Log("Right");
                    currentPageIndex++;
                    if (currentPageIndex >= pages.Length)
                    {
                        currentPageIndex = 0;
                    }
                    UpdatePageUI();
                }
            }
        }
    }

    private void HandleSaveNavigation()
    {
        //move left
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (selectedSaveButtonIndex == 1 || selectedSaveButtonIndex == 3)
            {
                selectedSaveButtonIndex--;
            }
        }
        //move right
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (selectedSaveButtonIndex == 0 || selectedSaveButtonIndex == 2)
            {
                selectedSaveButtonIndex++;
            }
        }
        //move up
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (selectedSaveButtonIndex == 2 || selectedSaveButtonIndex == 3)
            {
                selectedSaveButtonIndex = selectedSaveButtonIndex - 2;
            }
        }
        //move down
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (selectedSaveButtonIndex == 0 || selectedSaveButtonIndex == 1)
            {
                selectedSaveButtonIndex = selectedSaveButtonIndex + 2;
            }
        }
        //on submit
        else if (Input.GetButtonDown("Interact"))
        {
            if (selectedSaveButtonIndex == 0)
            {
                Save();
                saveStatus.sprite = saved;
            }
            else if (selectedSaveButtonIndex == 1)
            {
                NewSave();
                saveStatus.sprite = newSave;
            }
            else if (selectedSaveButtonIndex == 2)
            {
                Load();
                saveStatus.sprite = loaded;
            }
            else
            {
                saveMenu = false;
                SaveMenu.SetActive(false);
            }
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
            if(currentWorkstationName == "Cauldron")
            {
                Debug.Log("Combination submitted!");
                PerformCombinationAction();
                return; // Exit after handling the submit button
            }
            else if(currentWorkstationName == "Trashcan")
            {
                Debug.Log("Items trashed!");
                // Remove all items from workstation slots
                foreach (ItemSlot slot in workstationSlots)
                {
                    while (slot.quantity > 0)
                    {
                        slot.RemoveItem();
                    }
                    slot.SetHighlight(false);
                }
                return; // Exit after handling the submit button
            }
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
    
    private void UpdatePageUI()
    {
        TaskPage page = pages[currentPageIndex];

        characterImageUI.sprite = page.characterSprite;
        characterNameUI.text = page.characterName;
        if(pages[currentPageIndex].completed)
        {
            characterTextUI.text = page.characterTextAfter;
            // Get the Submit button GameObject
            Button submitButton = taskPanelButtons[1];
            // Find the child called "ButtonImage"
            Image buttonImage = submitButton.transform.Find("ButtonImage").GetComponent<Image>();
            // Swap the sprite
            buttonImage.sprite = doneSprite;
        }
        else
        {
            characterTextUI.text = page.characterTextBefore;
            // Get the Submit button GameObject
            Button submitButton = taskPanelButtons[1];
            // Find the child called "ButtonImage"
            Image buttonImage = submitButton.transform.Find("ButtonImage").GetComponent<Image>();
            // Swap the sprite
            buttonImage.sprite = submitSprite;
        }
        potionImageUI.sprite = page.potionSprite;
        potionNameUI.text = page.potionName;
        potionTextUI.text = page.potionText;
    }

    public void StartCraftingMinigame()
    {
        int sequenceLength = 3;
        List<string> possibleKeys = new List<string> { "Z", "X", "C", "V", "B", "N" };

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

    public void UpdateCraftingMinigame()
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

    public void FinishCraftingMinigame()
    {
        minigameActive = false;
        CraftingMinigamePanel.SetActive(false);

        // Highlight button and unhighlight the workstation slots
        for(int i = 0; i < 4; i++)
        {
            workstationSlots[i].SetHighlight(false);
        }
            
        workstationButton.SetHighlight(true); 
           
        //Debug.Log("Minigame complete!");
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
            // If no valid recipe, create Oops Potion
            AddItem("Oops Potion", 1, oopsPotionSprite, "Oops… did you mean to make this?");
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


