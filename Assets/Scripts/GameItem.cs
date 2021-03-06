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
    public string description = "";
    public Sprite texture;
    public int cost = 0;
    public bool isEquipable = false;
    public BodyPart bodyPart = BodyPart.NONE;
    public bool isEquipped = false;
    public int armorValue = 0;
    public bool isWeapon = false;
    public int damageValue = 0;
    public GameItemType gameItemType = GameItemType.NONE;
    public UsableEffects usableEffect = null;

    public ItemRequirement[] requirements = null;

    public GameItem()
    {
        ID = -1;
    }

    public GameItem(GameItem copy)
    {
        ID = copy.ID;
        quantity = copy.quantity;
        name = copy.name;
        description = copy.description;
        texture = copy.texture;
        cost = copy.cost;
        isEquipable = copy.isEquipable;
        bodyPart = copy.bodyPart;
        isEquipped = copy.isEquipped;
        armorValue = copy.armorValue;
        isWeapon = copy.isWeapon;
        damageValue = copy.damageValue;
        requirements = copy.requirements;
        gameItemType = copy.gameItemType;
        usableEffect = copy.usableEffect;
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

public enum GameItemType
{
    NONE,
    CLOTH,
    WEAPON,
    SHIELD,
    ITEM,
    QUEST
}

[System.Serializable]
public class UsableEffects
{
    public UsableItemType usableItemType = UsableItemType.NONE;
    public float value = 0;
}

public enum UsableItemType
{
    NONE,
    POTION,
    STARCLUE
}