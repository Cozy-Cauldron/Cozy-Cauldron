using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
   public string InteractionPrompt => prompt;

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
        //Debug.Log("Pick up!");
        int leftOverItems = inventoryManager.AddItem(itemName, quantity, itemImage, itemDescription);
        if(leftOverItems<=0)
        {
          Destroy(gameObject);
        }
        else
        {
          quantity = leftOverItems;
        }
        return true;
   }
    public Sprite GetItemImage()
    {
        return itemImage;
    }

}
