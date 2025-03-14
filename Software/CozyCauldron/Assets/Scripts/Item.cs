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
          inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
     }

   public bool Interact(Interactor interactor)
   {
        Debug.Log("Pick up!");
        inventoryManager.AddItem(itemName, quantity, itemImage, itemDescription);
        Destroy(gameObject);
        return true;
   }
    public Sprite GetItemImage()
    {
        return itemImage;
    }

}
