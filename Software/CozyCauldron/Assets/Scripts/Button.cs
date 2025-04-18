using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public GameObject SelectedPanel;

    public void SetHighlight(bool isHighlighted)
    {
        if (isHighlighted)
        {
            SelectedPanel.SetActive(true);
        }
        else
        {
            SelectedPanel.SetActive(false);
        }
    }
}
