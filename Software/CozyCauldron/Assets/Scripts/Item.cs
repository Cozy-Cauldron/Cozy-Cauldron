using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Collections;

public class Item : MonoBehaviour, IInteractable
{
     [SerializeField] public string itemName;

     [SerializeField] public int quantity;

     [SerializeField] public Sprite itemImage;

     [TextArea][SerializeField] public string itemDescription;

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
        if(inventoryManager.workstationActivated || inventoryManager.saveMenu || inventoryManager.taskPanelActivated)
        {
            Input.ResetInputAxes();
            return false;
        }

        Debug.Log("Interact() called on: " + gameObject.name + " with itemName: " + itemName);
        // Determine which animation to play
        string trigger = "";
        if (itemName == "Cauldron" || itemName == "Trashcan" || itemName == "Crystal Ball" || itemName == "Bed")
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
            playerMovement.canMove = false;
            playerMovement.m_Animator.SetTrigger(trigger);
        }

        // Start the coroutine to handle the delayed pick-up
        StartCoroutine(HandleDelayedPickup(trigger, playerMovement));

        return true;
    }

    private IEnumerator HandleDelayedPickup(string animationTrigger, PlayerMovement playerMovement)
    {
        // Wait 1 second for the animation to finish
        yield return new WaitForSeconds(1f);
        playerMovement.canMove = true;

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
        else if (itemName == "Bed")
        {
            inventoryManager.saveMenu = true;
        }
        else
        {
            bool matchingName = false;
            foreach (ItemSlot inventorySlot in inventoryManager.itemSlots)
            {
                if (inventorySlot.itemName == itemName)
                {
                    matchingName = true;
                }
            }
            //check if inventory full
            bool inventoryfull = true;
            foreach (ItemSlot inventorySlot in inventoryManager.itemSlots)
            {
                if (inventorySlot.quantity == 0)
                {
                    inventoryfull = false;
                }
            }
            if (!matchingName && inventoryfull)
            {
                string popupText = $"Inventory Full";
                inventoryManager.popup.text = popupText;
                inventoryManager.popup.gameObject.SetActive(true);
                StartCoroutine(inventoryManager.HidePopupAfterDelay());
                yield break;
            }

            // Add to inventory after animation
            if (animationTrigger == "StartFishing" || animationTrigger == "CatchBug")
            {
                inventoryManager.currentCraftingItem = this; // Save reference
                inventoryManager.StartCraftingMinigame();
            }
            else
            {
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
    }

    public Sprite GetItemImage()
    {
        return itemImage;
    }
}
