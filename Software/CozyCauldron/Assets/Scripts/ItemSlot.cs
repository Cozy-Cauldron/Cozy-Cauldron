using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour 
{
    //Item Data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;

    [SerializeField] private int maxNumberOfItems;
    [SerializeField] private Sprite emptySlotSprite;

    //Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    public GameObject SelectedPanel;

    //Item Description Slot
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;


    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        //Check to see if the slot is already full
        if(isFull)
        {
            return quantity;
        }
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        itemImage.sprite = itemSprite;

        //update quantity
        this.quantity += quantity;
        if(this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;
        
            //return the leftover items
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }

        //update quantitiy text
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        //no leftovers
        return 0;
    }

    public void SetHighlight(bool isHighlighted)
    {
       if (SelectedPanel != null)
        {
            SelectedPanel.SetActive(isHighlighted);
        }

        if (isHighlighted && this.quantity > 0) // Only update UI if the slot has an item
        {
            if (ItemDescriptionNameText != null) ItemDescriptionNameText.text = itemName;
            if (ItemDescriptionText != null) ItemDescriptionText.text = itemDescription;
            if (itemDescriptionImage != null) itemDescriptionImage.sprite = itemSprite;
        }
        else if (isHighlighted) // Clear description when selecting an empty slot
        {
            if (ItemDescriptionNameText != null) ItemDescriptionNameText.text = "";
            if (ItemDescriptionText != null) ItemDescriptionText.text = "";
            if (itemDescriptionImage != null) itemDescriptionImage.sprite = emptySlotSprite;
        }
    }

    public void RemoveItem()
    {
        //decrement quantity
        //if quantity is 0
        //set isFull to false and clear slot
       if(this.quantity > 0)
       {
            this.quantity--;
            this.isFull = false;
            this.quantityText.text = this.quantity.ToString();
       }
       if(this.quantity <= 0)
       {
            this.quantity = 0;
            this.itemName = "";
            this.itemSprite = null;
            this.itemDescription = null;
            itemImage.sprite = emptySlotSprite;
            quantityText.enabled = false;

            SetHighlight(true);

        }
    }
}
