using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Collections;

public class Item : MonoBehaviour, IInteractable
{
     [SerializeField] private string itemName;

     [SerializeField] private int quantity;

     [SerializeField] private Sprite itemImage;

     [TextArea][SerializeField] private string itemDescription;

     private InventoryManager inventoryManager;

     void Start()
     {
        GameObject inventoryObject = GameObject.Find("InventoryCanvas");
    
        if (inventoryObject == null)
        {
            Debug.LogError("Item: Could not find InventoryCanvas in the scene!");
            return;
        }

        inventoryManager = inventoryObject.GetComponent<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("Item: InventoryCanvas found, but it has no InventoryManager component!");
        }
     }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interact() called on: " + gameObject.name + " with itemName: " + itemName);

        // Determine which animation to play
        string trigger = "";
        if(itemName == "Bed" && inventoryManager.saveMenuJustOpened)
        {
            return false; // Don't interact if the save menu was just opened
        }
        else if (itemName == "Cauldron" || itemName == "Trashcan" || itemName == "Crystal Ball" || itemName == "Bed")
        {
            trigger = "Craft";
        }
        else if (itemName == "Clownfish" || itemName == "Sturgeon" || itemName == "Bass" ||
            itemName == "Salmon" || itemName == "Butterfly Fish" || itemName == "Goldfish" ||
            itemName == "Pufferfish")
        {
            trigger = "StartFishing";
        }
        else if (itemName == "Jumping Spider" || itemName == "Lady Bug" || itemName == "Beetle" ||
                itemName == "Roly Poly")
        {
            trigger = "CatchBug";
        }
        else
        {
            trigger = "PickUp";
        }

        // Trigger the animation
        PlayerMovement playerMovement = interactor.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.m_Animator.SetTrigger(trigger);
        }

        // Start the coroutine to handle the delayed pick-up
        StartCoroutine(HandleDelayedPickup(trigger));

        return true;
    }

    private IEnumerator HandleDelayedPickup(string animationTrigger)
    {
        // Wait 1 second for the animation to finish
        yield return new WaitForSeconds(1f);

        // Now actually pick up the item (or do fishing/bug logic)
        if (itemName == "Cauldron" || itemName == "Trashcan")
        {
            Debug.Log("Interacted with " + itemName);
            inventoryManager.workstationActivated = true;
            inventoryManager.currentWorkstationName = itemName;
            inventoryManager.currentWorkstationSprite = itemImage;
        }
        else if (itemName == "Crystal Ball")
        {
            inventoryManager.taskPanelActivated = true;
        }
        else if(itemName == "Bed" && inventoryManager.saveMenuJustOpened)
        {
            StartCoroutine(ResetSaveMenuJustOpened());
        }
        else if (itemName == "Bed" && !inventoryManager.saveMenuJustOpened)
        {
            inventoryManager.saveMenuJustOpened = true;
            inventoryManager.saveMenu = true;
        }
        else
        {
            // Add to inventory after animation
            if (animationTrigger == "StartFishing" || animationTrigger == "CatchBug")
            {
                inventoryManager.StartCraftingMinigame();
            }
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, itemImage, itemDescription);
            if (leftOverItems <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                quantity = leftOverItems;
            }
        }
    }

    public Sprite GetItemImage()
    {
        return itemImage;
    }

    private IEnumerator ResetSaveMenuJustOpened()
    {
        // Wait one frame so input isn't double-counted
        yield return null;
        inventoryManager.saveMenuJustOpened = false;
    }
}
