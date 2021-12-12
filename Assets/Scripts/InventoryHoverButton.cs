using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text descriptionText = null;
    public string description = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionText != null)
            descriptionText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(descriptionText != null)
            descriptionText.text = "";
    }
}
