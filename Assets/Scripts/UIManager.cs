using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; set; } = null;

    public Button townButton = null;
    public Button mapButton = null;
    public Button inventoryButton = null;
    public Button craftButton = null;
    public Button starsButton = null;
    private Button lastButton = null;

    public RectTransform townPage = null;
    public RectTransform inventoryPage = null;
    public RectTransform craftPage = null;
    public RectTransform castlePage = null;
    public RectTransform starsPage = null;

    public Text townDialogText = null;
    public GameObject townDialogGameObject = null;
    public GameObject characterGameObject = null;

    private RectTransform pageToShow = null;
    private RectTransform nextPageToShow = null;
    private bool mustShow = true;

    private const float timeToShow = 0.5f;

    private bool isSwitching = false;

    public Text houseText = null;

    public GameObject choices = null;

    [HideInInspector]
    public House currentHouse = null;

    public Transform inventoryShop = null;
    public Transform inventoryShopBuying = null;
    public GameObject inventoryShopGameObject = null;
    public GameObject inventoryButtonPrefab = null;
    public Text shopItemDesc = null;
    public Text buyItemDesc = null;

    public Text inventoryGoldShopCount = null;

    List<GameItem> buying = new List<GameItem>();
    public Text costText = null;
    private int cost = 0;
    public Button buyButton = null;

    public Transform inventoryPanel = null;
    public Text inventoryLevelCount = null;
    public Text inventoryDefenseCount = null;
    public Text inventoryAttackCount = null;
    private Button headItemButton = null;
    private Button bodyItemButton = null;
    private Button legsItemButton = null;
    private Button weaponItemButton = null;
    public Text descriptionText = null;

    public Button inventoryClothTab = null;
    public Button inventoryWeaponTab = null;
    public Button inventoryShieldTab = null;
    public Button inventoryItemTab = null;
    public Button inventoryQuestTab = null;

    public Slider lifeSlider = null;
    public Slider manaSlider = null;
    public Slider xpSlider = null;

    private const float borderSize = 40;

    private void Awake()
    {
        instance = this;
        lastButton = mapButton;

		Vector2 vec = (-Vector2.right * townPage.rect.width) - Vector2.right * borderSize * 2;
        townPage.anchoredPosition = vec;
        inventoryPage.anchoredPosition = vec;
        craftPage.anchoredPosition = vec;
        castlePage.anchoredPosition = vec;
        starsPage.anchoredPosition = vec;
    }

    private void Start()
    {
        UpdateGoldCount();
        lifeSlider.value = InventoryManager.instance.life / InventoryManager.instance.maxLife;
        lifeSlider.GetComponentInChildren<Text>().text = "Life : " + InventoryManager.instance.life + " / " + InventoryManager.instance.maxLife;
        manaSlider.value = InventoryManager.instance.mana / InventoryManager.instance.maxMana;
        manaSlider.GetComponentInChildren<Text>().text = "Mana : " + InventoryManager.instance.mana + " / " + InventoryManager.instance.maxMana;
    }

    public void BlockTown(bool newState)
    {
        townButton.interactable = !newState;
    }

    public void ActivatePage(int page)
    {
        if (isSwitching)
            return;

        if (lastButton != null)
        {
            lastButton.interactable = true;
        }

        if (page == 0)
        {
            nextPageToShow = townPage;
            townButton.interactable = false;
            lastButton = townButton;
        }
        else if (page == 1)
        {
            nextPageToShow = inventoryPage;
            inventoryButton.interactable = false;
            lastButton = inventoryButton;
            ShowInventory(GameItemType.CLOTH);
        }
        else if (page == 2)
        {
            nextPageToShow = craftPage;
            craftButton.interactable = false;
            lastButton = craftButton;
            CraftManager.instance.UpdateCraftList();
        }
        else if(page == 3)
        {
            nextPageToShow = starsPage;
            starsButton.interactable = false;
            lastButton = starsButton;
        }
        else if(page == -1)
        {
            mapButton.interactable = false;
            lastButton = mapButton;
        }
        MusicManager.instance.PlayActionSound();
        HiddeAllPages();
    }

    public void RemoveEquippedItem(GameItemType aType, BodyPart aPart = BodyPart.NONE)
    {
        GameItem anItem = null;
        switch(aType)
        {
            case GameItemType.CLOTH:
                {
                    switch(aPart)
                    {
                        case BodyPart.HEAD:
                            {
                                anItem = InventoryManager.instance.headItem;
                                break;
                            }
                        case BodyPart.BODY:
                            {
                                anItem = InventoryManager.instance.bodyItem;
                                break;
                            }
                        case BodyPart.LEGS:
                            {
                                anItem = InventoryManager.instance.legsItem;
                                break;
                            }
                    }
                    break;
                }
            case GameItemType.WEAPON:
                {
                    anItem = InventoryManager.instance.weaponItem;
                    if (anItem != null)
                    {
                        MusicManager.instance.PlayActionSound();
                        InventoryManager.instance.weaponItem = null;
                        InventoryManager.instance.damage -= anItem.damageValue;
                        UpdateAttackCount();
                        if (weaponItemButton != null)
                        {
                            weaponItemButton.image.color = Color.white;
                            weaponItemButton = null;
                        }
                    }
                    return;
                }
        }

        if (anItem == null)
            return;

        anItem.isEquipped = false;

        switch (anItem.bodyPart)
        {
            case BodyPart.HEAD:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.headItem = null;
                    InventoryManager.instance.defense -= anItem.armorValue;
                    if (headItemButton != null)
                    {
                        headItemButton.image.color = Color.white;
                        headItemButton = null;
                    }
                    break;
                }
            case BodyPart.BODY:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.bodyItem = null;
                    InventoryManager.instance.defense -= anItem.armorValue;
                    if (bodyItemButton != null)
                    {
                        bodyItemButton.image.color = Color.white;
                        bodyItemButton = null;
                    }
                    break;
                }
            case BodyPart.LEGS:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.legsItem = null;
                    InventoryManager.instance.defense -= anItem.armorValue;
                    if (legsItemButton != null)
                    {
                        legsItemButton.image.color = Color.white;
                        legsItemButton = null;
                    }
                    break;
                }
        }
        UpdateDefenseCount();
    }

    public void EquipItem(GameItem anItem, Button buttonClicked)
    {
        switch(anItem.gameItemType)
        {
            case GameItemType.CLOTH:
                {
                    InventoryManager.instance.defense += anItem.armorValue;
                    switch (anItem.bodyPart)
                    {
                        case BodyPart.HEAD:
                            {
                                InventoryManager.instance.headItem = anItem;
                                headItemButton = buttonClicked;
                                break;
                            }
                        case BodyPart.BODY:
                            {
                                InventoryManager.instance.bodyItem = anItem;
                                bodyItemButton = buttonClicked;
                                break;
                            }
                        case BodyPart.LEGS:
                            {
                                InventoryManager.instance.legsItem = anItem;
                                legsItemButton = buttonClicked;
                                break;
                            }
                    }
                    UpdateDefenseCount();
                    break;
                }
            case GameItemType.WEAPON:
                {
                    InventoryManager.instance.weaponItem = anItem;
                    InventoryManager.instance.damage += anItem.damageValue;
                    weaponItemButton = buttonClicked;
                    UpdateAttackCount();
                    break;
                }
        }
        
        buttonClicked.image.color = Color.red;
        anItem.isEquipped = true;
    }
    //Type : 1 = Clothes, 2 = Weapons, 3 = Shields, 4 = Items, 5 = Quests
    public void ShowInventory(int aType)
    {
        ShowInventory((GameItemType)aType);
    }

    //Type : 1 = Clothes, 2 = Weapons, 3 = Shields, 4 = Items, 5 = Quests
    public void ShowInventory(GameItemType aType)
    {
        inventoryClothTab.image.color = Color.white;
        inventoryWeaponTab.image.color = Color.white;
        inventoryShieldTab.image.color = Color.white;
        inventoryItemTab.image.color = Color.white;
        inventoryQuestTab.image.color = Color.white;

        switch (aType)
        {
            case GameItemType.CLOTH:
                inventoryClothTab.image.color = Color.red;
                break;
            case GameItemType.WEAPON:
                inventoryWeaponTab.image.color = Color.red;
                break;
            case GameItemType.SHIELD:
                inventoryShieldTab.image.color = Color.red;
                break;
            case GameItemType.ITEM:
                inventoryItemTab.image.color = Color.red;
                break;
            case GameItemType.QUEST:
                inventoryQuestTab.image.color = Color.red;
                break;
        }

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            Destroy(inventoryPanel.GetChild(i).gameObject);
        }

        foreach(KeyValuePair<int,GameItem> item in InventoryManager.instance.inventory)
        {
            if (item.Value.gameItemType != aType)
                continue;

            RectTransform rect = Instantiate(inventoryButtonPrefab, inventoryPanel).GetComponent<RectTransform>();

            string equipableText = "";
            if (item.Value.isEquipable)
            {
                switch (item.Value.bodyPart)
                {
                    case BodyPart.BODY:
                        equipableText = "{ Body } \n Armor : " + item.Value.armorValue;
                        break;
                    case BodyPart.HEAD:
                        equipableText = "{ Head } \n Armor : " + item.Value.armorValue;
                        break;
                    case BodyPart.LEGS:
                        equipableText = "{ Legs } \n Armor : " + item.Value.armorValue;
                        break;
                }
            }
            else if (item.Value.isWeapon)
            {
                equipableText = "{ Weapon } \n Damage : " + item.Value.damageValue;
            }
            rect.GetComponent<InventoryHoverButton>().description =
                item.Value.name + "\n Quantity : " +
                item.Value.quantity + "\n Cost : " +
                item.Value.cost + "\n Total : " + (item.Value.cost * item.Value.quantity) +
                (equipableText != "" ? "\n" + equipableText : "");
            rect.GetComponent<InventoryHoverButton>().descriptionText = descriptionText;

            rect.transform.GetChild(1).GetComponent<Image>().sprite = item.Value.texture;

            if (item.Value.isEquipped)
            {
                rect.GetComponent<Button>().image.color = Color.red;
            }

            if (item.Value.isEquipable)
            {
                switch(item.Value.bodyPart)
                {
                    case BodyPart.HEAD:
                        {
                            rect.GetComponent<Button>().onClick.AddListener(delegate {
                                MusicManager.instance.PlayActionSound();
                                if (InventoryManager.instance.headItem != null)
                                {
                                    if (InventoryManager.instance.headItem != null)
                                    {
                                        GameItem itemTemp = InventoryManager.instance.headItem;
                                        RemoveEquippedItem(GameItemType.CLOTH, BodyPart.HEAD);
                                        if (itemTemp.ID == item.Value.ID)
                                            return;
                                    }
                                }
                                EquipItem(item.Value, rect.GetComponent<Button>());
                            });
                            break;
                        }
                    case BodyPart.BODY:
                        {
                            rect.GetComponent<Button>().onClick.AddListener(delegate {
                                MusicManager.instance.PlayActionSound();
                                if (InventoryManager.instance.bodyItem != null)
                                {
                                    if (InventoryManager.instance.bodyItem != null)
                                    {
                                        GameItem itemTemp = InventoryManager.instance.bodyItem;
                                        RemoveEquippedItem(GameItemType.CLOTH, BodyPart.BODY);
                                        if (itemTemp.ID == item.Value.ID)
                                            return;
                                    }
                                }
                                EquipItem(item.Value, rect.GetComponent<Button>());
                            });
                            break;
                        }
                    case BodyPart.LEGS:
                        {
                            rect.GetComponent<Button>().onClick.AddListener(delegate {
                                MusicManager.instance.PlayActionSound();
                                if (InventoryManager.instance.legsItem != null)
                                {
                                    GameItem itemTemp = InventoryManager.instance.legsItem;
                                    RemoveEquippedItem(GameItemType.CLOTH, BodyPart.LEGS);
                                    if (itemTemp.ID == item.Value.ID)
                                        return;
                                }
                                EquipItem(item.Value, rect.GetComponent<Button>());
                            });
                            break;
                        }
                }
            }
            else if(item.Value.isWeapon)
            {
                rect.GetComponent<Button>().onClick.AddListener(delegate {
                    MusicManager.instance.PlayActionSound();
                    if (InventoryManager.instance.weaponItem != null)
                    {
                        GameItem itemTemp = InventoryManager.instance.weaponItem;
                        RemoveEquippedItem(GameItemType.WEAPON);
                        if (itemTemp.ID == item.Value.ID)
                            return;
                    }
                    EquipItem(item.Value, rect.GetComponent<Button>());
                });
            }
        }

        UpdateDefenseCount();
        UpdateAttackCount();
        lifeSlider.value = InventoryManager.instance.life / 100;
        lifeSlider.GetComponentInChildren<Text>().text = "Life : " + InventoryManager.instance.life + " / 100";
        manaSlider.value = InventoryManager.instance.mana / 100;
        manaSlider.GetComponentInChildren<Text>().text = "Mana : " + InventoryManager.instance.mana + " / 100";
        inventoryLevelCount.text = "Level : " + InventoryManager.instance.level;
        xpSlider.value = InventoryManager.instance.xp / InventoryManager.instance.xpToNextLevel;
        xpSlider.GetComponentInChildren<Text>().text = "Exp : " + InventoryManager.instance.xp.ToString("F2") + " / " + InventoryManager.instance.xpToNextLevel;
    }

    #region SHOP
    public void ShowShopInventory(bool aState)
    {
        inventoryShopGameObject.SetActive(aState);
    }

    public void AddItemToBuy(GameItem anItem)
    {
        MusicManager.instance.PlayActionSound();
        bool added = false;
        for(int i = 0; i < buying.Count; i++)
        {
            if(buying[i].ID == anItem.ID)
            {
                buying[i].quantity++;
                cost += anItem.cost;
                costText.text = "Cost : " + cost;
                added = true;
                break;
            }
        }
        if(!added)
        {
            buying.Add(new GameItem(anItem));
            cost += anItem.cost;
            costText.text = "Cost : " + cost;
        }

        if (cost > InventoryManager.instance.currentGold)
            buyButton.interactable = false;
        else
            buyButton.interactable = true;

        FillShopBuying();
    }

    public void RemoveItemToBuy(int anID)
    {
        MusicManager.instance.PlayActionSound();
        for (int i = 0; i < buying.Count; i++)
        {
            if (buying[i].ID == anID)
            {
                buying[i].quantity--;
                cost -= buying[i].cost;
                costText.text = "Cost : " + cost;
                if(buying[i].quantity <= 0)
                {
                    buying.RemoveAt(i);
                }
                break;
            }
        }

        if (cost > InventoryManager.instance.currentGold)
            buyButton.interactable = false;
        else
            buyButton.interactable = true;

        FillShopBuying();
    }

    public void BackFromShopInventory()
    {
        MusicManager.instance.PlayActionSound();
        choices.SetActive(true);
        inventoryShopGameObject.SetActive(false);
        buying.Clear();
        for (int i = 0; i < inventoryShopBuying.childCount; i++)
        {
            Destroy(inventoryShopBuying.GetChild(i).gameObject);
        }
        cost = 0;
        costText.text = "Cost : " + cost;
    }
    
    public void FillShopInventory(List<InventoryItem> items)
    {
        for (int i = 0; i < inventoryShop.childCount; i++)
        {
            Destroy(inventoryShop.GetChild(i).gameObject);
        }
        for (int i = 0; i < items.Count; i++)
        {
            GameItem gameItem = GameManager.instance.itemsData.GetGameItemWithID(items[i].ID);
            if (gameItem == null)
                continue;

            RectTransform rect = Instantiate(inventoryButtonPrefab, inventoryShop).GetComponent<RectTransform>();
            string equipableText = "";
            if (gameItem.isEquipable)
            {
                switch (gameItem.bodyPart)
                {
                    case BodyPart.BODY:
                        equipableText = "{ Body }";
                        break;
                    case BodyPart.HEAD:
                        equipableText = "{ Head }";
                        break;
                    case BodyPart.LEGS:
                        equipableText = "{ Legs }";
                        break;
                }
            }
            else if (gameItem.isWeapon)
            {
                equipableText = "{ Weapon }";
            }
            //rect.GetComponentInChildren<Text>().text = gameItem.name + " X " + items[i].quantity + "\t Cost :" + gameItem.cost + (equipableText != "" ? "\t" + equipableText : "");
            rect.GetChild(1).GetComponent<Image>().sprite = gameItem.texture;
            rect.GetComponent<InventoryHoverButton>().descriptionText = shopItemDesc;
            rect.GetComponent<InventoryHoverButton>().description = gameItem.name + " X " + items[i].quantity + "\t Cost :" + gameItem.cost + (equipableText != "" ? "\t" + equipableText : ""); ;
            GameItem temp = gameItem;
            rect.GetComponent<Button>().onClick.AddListener(delegate { AddItemToBuy(temp); });
            rect.transform.GetChild(1).GetComponent<Image>().sprite = gameItem.texture;
        }
    }

    public void FillShopBuying()
    {
        for(int i = 0; i < inventoryShopBuying.childCount; i++)
        {
            Destroy(inventoryShopBuying.GetChild(i).gameObject);
        }
        //inventoryShop.GetComponent<RectTransform>().sizeDelta = new Vector2(0, buying.Count * 30);
        for (int i = 0; i < buying.Count; i++)
        {
            RectTransform rect = Instantiate(inventoryButtonPrefab, inventoryShopBuying).GetComponent<RectTransform>();
            //rect.GetComponentInChildren<Text>().text = buying[i].name + " X " + buying[i].quantity + "\t Cost : " + buying[i].cost + "\t Total : " + (buying[i].cost * buying[i].quantity);
            rect.GetChild(1).GetComponent<Image>().sprite = buying[i].texture;
            rect.GetComponent<InventoryHoverButton>().descriptionText = buyItemDesc;
            rect.GetComponent<InventoryHoverButton>().description = buying[i].name + " X " + buying[i].quantity + "\t Cost : " + buying[i].cost + "\t Total : " + (buying[i].cost * buying[i].quantity);
            int id = buying[i].ID;
            rect.GetComponent<Button>().onClick.AddListener(delegate { RemoveItemToBuy(id); });
            rect.transform.GetChild(1).GetComponent<Image>().sprite = buying[i].texture;
        }
    }

    public void Buy()
    {
        MusicManager.instance.PlayActionSound();
        for (int i = 0; i < buying.Count; i++)
        {
            InventoryManager.instance.currentGold -= buying[i].cost;
            InventoryManager.instance.AddItem(buying[i]);
        }
        UpdateGoldCount();
        BackFromShopInventory();
    }
    #endregion
    public void UpdateGoldCount()
    {
        inventoryGoldShopCount.text = "Gold : " + InventoryManager.instance.currentGold.ToString("F0");
    }

    public void UpdateDefenseCount()
    {
        inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
    }

    private void UpdateAttackCount()
    {
        inventoryAttackCount.text = "Damage : " + InventoryManager.instance.damage;
    }

    #region PAGES
    private void HiddeAllPages()
    {
        StartCoroutine("SwitchPage");
    }

    private IEnumerator SwitchPage()
    {
        if(mustShow && (pageToShow != null || nextPageToShow != null))
        {
            GameManager.instance.isInMenu = true;
            isSwitching = true;
            pageToShow = nextPageToShow != null ? nextPageToShow : pageToShow;
            nextPageToShow = null;
            while (pageToShow.anchoredPosition.x < 0)
            {
                pageToShow.anchoredPosition += Vector2.right * Time.deltaTime * ((pageToShow.rect.width + (borderSize * 2)) / timeToShow);
                yield return null;
            }
            
            mustShow = false;
            pageToShow.anchoredPosition = new Vector2(0, pageToShow.anchoredPosition.y);

            isSwitching = false;
        }
        else if (!mustShow && pageToShow != null)
        {
            isSwitching = true;
            while (pageToShow.anchoredPosition.x > -pageToShow.rect.width - (borderSize * 2))
            {
                pageToShow.anchoredPosition -= Vector2.right * Time.deltaTime * ((pageToShow.rect.width - (borderSize * 2)) / timeToShow);
                yield return null;
            }

            pageToShow = null;
            mustShow = true;
            if(nextPageToShow != null)
            {
                pageToShow = nextPageToShow;
                nextPageToShow = null;
                StartCoroutine("SwitchPage");
            }
            else
            {
                isSwitching = false;
                GameManager.instance.isInMenu = false;
            }
        }
    }

    public void CloseHouse()
    {
        currentHouse.Close();
    }

    public void SetTownDialogText(string aText)
    {
        townDialogText.text = aText;
    }

    public void SetHouseName(string aName)
    {
        houseText.text = aName;
    }

    public void ShowCharacter(bool aState)
    {
        characterGameObject.SetActive(aState);
    }

    public void ShowTownDialogGameObject(bool aState)
    {
        townDialogGameObject.SetActive(aState);
    }
    #endregion
}
