using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text descriptionText = null;
    public string description = "";
    public GameItem anItem = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        string equipableText = "";
        if (anItem.isEquipable)
        {
            switch (anItem.bodyPart)
            {
                case BodyPart.BODY:
                    equipableText = "{ Body } \n Armor : " + anItem.armorValue;
                    break;
                case BodyPart.HEAD:
                    equipableText = "{ Head } \n Armor : " + anItem.armorValue;
                    break;
                case BodyPart.LEGS:
                    equipableText = "{ Legs } \n Armor : " + anItem.armorValue;
                    break;
            }
        }
        else if (anItem.isWeapon)
        {
            equipableText = "{ Weapon } \n Damage : " + anItem.damageValue;
        }
        description =
            anItem.name + "\n Quantity : " +
            anItem.quantity + "\n Cost : " +
            anItem.cost + "\n Total : " + (anItem.cost * anItem.quantity) +
            (equipableText != "" ? "\n" + equipableText : "");

        if (descriptionText != null)
            descriptionText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(descriptionText != null)
            descriptionText.text = "";
    }
}
