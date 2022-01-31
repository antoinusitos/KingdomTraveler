using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance = null;

    public Dictionary<int, GameItem> inventory = new Dictionary<int, GameItem>();

    public float currentGold = 0;

    public GameItem headItem = null;
    public GameItem bodyItem = null;
    public GameItem legsItem = null;
    public GameItem weaponItem = null;

    public List<InventoryItem> defaultItems = new List<InventoryItem>();

    public float life = 100;
    public float maxLife = 100;
    public float mana = 100;
    public float maxMana = 100;
    public float xp = 0;
    public float xpToNextLevel = 10;
    public float damage = 0;
    public float defense = 0;
    public int level = 1;

    public float horseSpeed = 2.0f;

    public Slider playerLifeSlider = null;
    public Text playerLifeSliderText = null;

    public GameObject receiveItemPanel = null;
    public Text receiveItemTitle = null;
    public Text receiveItemDesc = null;
    public Image receiveItemImage = null;
    private bool isWaitingForInput = false;
    private bool itemJustAdded = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < defaultItems.Count; i++)
        {
            GameItem gameItem = GameManager.instance.itemsData.GetGameItemWithID(defaultItems[i].ID);
            if (gameItem == null)
                continue;

            for(int j = 0; j < defaultItems[i].quantity; j++)
            {
                AddItem(gameItem);
            }
        }

        isWaitingForInput = false;
        receiveItemPanel.SetActive(false);

        headItem = null;
        bodyItem = null;
        legsItem = null;
        weaponItem = null;
    }

    public bool GetItemJustAdded()
    {
        if(itemJustAdded)
        {
            itemJustAdded = false;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(20);
        }

        if(isWaitingForInput && InputManager.instance.GetInputDone())
        {
            ItemJustAdded();
        }
    }

    private void ItemJustAdded()
    {
        isWaitingForInput = false;
        receiveItemPanel.SetActive(false);
        itemJustAdded = true;
    }

    public void AddItem(GameItem anItem)
    {
        receiveItemPanel.SetActive(true);
        receiveItemTitle.text = anItem.name;
        receiveItemDesc.text = anItem.description;
        receiveItemImage.sprite = anItem.texture;
        isWaitingForInput = true;
        if (inventory.ContainsKey(anItem.ID))
        {
            inventory[anItem.ID].quantity += anItem.quantity;
        }
        else
        {
            inventory.Add(anItem.ID, new GameItem(anItem));
        }
    }

    public void RemoveItem(GameItem anItem, int quantity)
    {
        if (inventory.ContainsKey(anItem.ID))
        {
            inventory[anItem.ID].quantity -= quantity;
            if(inventory[anItem.ID].quantity <= 0)
            {
                inventory.Remove(anItem.ID);
            }
        }
    }

    public void RemoveItem(int anID, int quantity)
    {
        if (inventory.ContainsKey(anID))
        {
            inventory[anID].quantity -= quantity;
            if (inventory[anID].quantity <= 0)
            {
                inventory.Remove(anID);
            }
        }
    }

    public void AddXP(float aValue)
    {
        xp += aValue;
        CheckLevelUp();
    }

    public void AddGold(float aValue)
    {
        receiveItemPanel.SetActive(true);
        receiveItemTitle.text = "Gold x " + (int)aValue;
        receiveItemDesc.text = "Basic resource to trade";
        InputManager.instance.SetWaitingForInput();
        StartCoroutine(AddGoldAnim(aValue));
    }

    private IEnumerator AddGoldAnim(float aValue)
    {
        while(!InputManager.instance.GetInputDone())
        {
            yield return null;
        }

        float before = currentGold;
        float timer = 0;
        while(timer < 1)
        {
            currentGold = Mathf.Lerp(before, before + aValue, timer);
            timer += Time.deltaTime;
            UIManager.instance.UpdateGoldCount();
            yield return null;
        }
        currentGold = before + aValue;
        UIManager.instance.UpdateGoldCount();
        ItemJustAdded();
    }

    public void CheckLevelUp()
    {
        if (xp >= xpToNextLevel)
        {
            float toReport = xp - xpToNextLevel;
            level++;
            xp = 0;
            xpToNextLevel *= 2;
            AddXP(toReport);
        }
    }

    public int GetItemQuantity(int anID)
    {
        int quantity = 0;

        if(inventory.ContainsKey(anID))
        {
            quantity = inventory[anID].quantity;
        }

        return quantity;
    }

    public void SetItemQuantity(int anID, int aQuantity)
    {
        if (inventory.ContainsKey(anID))
        {
            if (aQuantity <= 0)
                inventory.Remove(anID);
            else
                inventory[anID].quantity = aQuantity;
        }
    }

    public void TakeDamage(float aDamage)
    {
        life -= aDamage;
        RefreshLifeStatus();
    }

    public void RefreshLifeStatus()
    {
        playerLifeSlider.value = life / maxLife;
        playerLifeSliderText.text = "Life : " + life.ToString("F2") + " / " + maxLife;
    }
}
