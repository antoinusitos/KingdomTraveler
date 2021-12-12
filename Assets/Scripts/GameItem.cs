using UnityEngine;

[System.Serializable] 
public class ItemRequirement
{
    public int ID = -1;
    public int quantity = 0;
}

[System.Serializable]
public class GameItem
{
    public int ID = -1;
    public int quantity = 0;
    public string name = "";
    public Sprite texture;
    public int cost = 0;
    public bool isEquipable = false;
    public BodyPart bodyPart = BodyPart.NONE;
    public bool isEquipped = false;
    public int armorValue = 0;
    public bool isWeapon = false;
    public int damageValue = 0;

    public ItemRequirement[] requirements = null;

    public GameItem(GameItem copy)
    {
        ID = copy.ID;
        quantity = copy.quantity;
        name = copy.name;
        texture = copy.texture;
        cost = copy.cost;
        isEquipable = copy.isEquipable;
        bodyPart = copy.bodyPart;
        isEquipped = copy.isEquipped;
        armorValue = copy.armorValue;
        isWeapon = copy.isWeapon;
        damageValue = copy.damageValue;
        requirements = copy.requirements;
    }
}

public enum BodyPart
{
    NONE,
    HEAD,
    BODY,
    LEGS
}

[System.Serializable]
public class InventoryItem
{
    public int ID = -1;
    public int quantity = 0;
}
