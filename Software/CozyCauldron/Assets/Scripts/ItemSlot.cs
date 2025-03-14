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

    //Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    public GameObject SelectedPanel;

    //Item Description Slot
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;


    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;

    }

    public void SetHighlight(bool isHighlighted)
    {
       if (SelectedPanel != null)
        {
            SelectedPanel.SetActive(isHighlighted);
        }

        if (isHighlighted && isFull) // Only update UI if the slot has an item
        {
            if (ItemDescriptionNameText != null) ItemDescriptionNameText.text = itemName;
            if (ItemDescriptionText != null) ItemDescriptionText.text = itemDescription;
            if (itemDescriptionImage != null) itemDescriptionImage.sprite = itemSprite;
        }
        else if (isHighlighted) // Clear description when selecting an empty slot
        {
            if (ItemDescriptionNameText != null) ItemDescriptionNameText.text = "";
            if (ItemDescriptionText != null) ItemDescriptionText.text = "";
            if (itemDescriptionImage != null) itemDescriptionImage.sprite = null;
        }
    }
}
