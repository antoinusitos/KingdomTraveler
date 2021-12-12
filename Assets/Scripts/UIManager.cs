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
    public Button headItemButton = null;
    public Button bodyItemButton = null;
    public Button legsItemButton = null;
    public Button weaponItemButton = null;
    public Text descriptionText = null;

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
            ShowInventory();
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

    public void RemoveEquippedItem(int anIndex)
    {
        GameItem anItem = null;
        if (anIndex == 1)
            anItem = InventoryManager.instance.headItem;
        else if (anIndex == 2)
            anItem = InventoryManager.instance.bodyItem;
        else if (anIndex == 3)
            anItem = InventoryManager.instance.legsItem;
        else if (anIndex == 4)
        {
            anItem = InventoryManager.instance.weaponItem;
            if (anItem != null)
            {
                MusicManager.instance.PlayActionSound();
                InventoryManager.instance.weaponItem = null;
                weaponItemButton.transform.GetChild(1).GetComponent<Image>().sprite = null;
                weaponItemButton.GetComponent<InventoryHoverButton>().description = "";
                descriptionText.text = "";
                InventoryManager.instance.damage -= anItem.damageValue;
                inventoryAttackCount.text = "Damage : " + InventoryManager.instance.damage;
            }
            return;
        }

        if (anItem == null)
            return;

        switch (anItem.bodyPart)
        {
            case BodyPart.HEAD:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.headItem = null;
                    headItemButton.transform.GetChild(1).GetComponent<Image>().sprite = null;
                    headItemButton.GetComponent<InventoryHoverButton>().description = "";
                    descriptionText.text = "";
                    InventoryManager.instance.defense -= anItem.armorValue;
                    inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
                    break;
                }
            case BodyPart.BODY:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.bodyItem = null;
                    bodyItemButton.transform.GetChild(1).GetComponent<Image>().sprite = null;
                    bodyItemButton.GetComponent<InventoryHoverButton>().description = "";
                    descriptionText.text = "";
                    InventoryManager.instance.defense -= anItem.armorValue;
                    inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
                    break;
                }
            case BodyPart.LEGS:
                {
                    MusicManager.instance.PlayActionSound();
                    InventoryManager.instance.legsItem = null;
                    legsItemButton.transform.GetChild(1).GetComponent<Image>().sprite = null;
                    legsItemButton.GetComponent<InventoryHoverButton>().description = "";
                    descriptionText.text = "";
                    InventoryManager.instance.defense -= anItem.armorValue;
                    inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
                    break;
                }
        }
    }

    public void ShowInventory()
    {
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            Destroy(inventoryPanel.GetChild(i).gameObject);
        }

        foreach(KeyValuePair<int,GameItem> item in InventoryManager.instance.inventory)
        {
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
                                    InventoryManager.instance.defense -= InventoryManager.instance.headItem.armorValue;
                                }
                                InventoryManager.instance.headItem = item.Value; 
                                headItemButton.transform.GetChild(1).GetComponent<Image>().sprite = item.Value.texture;
                                headItemButton.GetComponent<InventoryHoverButton>().descriptionText = descriptionText;
                                headItemButton.GetComponent<InventoryHoverButton>().description =
                                item.Value.name + "\n Quantity : " +
                                item.Value.quantity + "\n Cost : " +
                                item.Value.cost + "\n Total : " + (item.Value.cost * item.Value.quantity) +
                                (equipableText != "" ? "\n" + equipableText : "");
                                InventoryManager.instance.defense += item.Value.armorValue;
                                inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
                            });
                            break;
                        }
                    case BodyPart.BODY:
                        {
                            rect.GetComponent<Button>().onClick.AddListener(delegate {
                                MusicManager.instance.PlayActionSound();
                                if (InventoryManager.instance.bodyItem != null)
                                {
                                    InventoryManager.instance.defense -= InventoryManager.instance.bodyItem.armorValue;
                                }
                                InventoryManager.instance.bodyItem = item.Value; 
                                bodyItemButton.transform.GetChild(1).GetComponent<Image>().sprite = item.Value.texture;
                                bodyItemButton.GetComponent<InventoryHoverButton>().descriptionText = descriptionText;
                                bodyItemButton.GetComponent<InventoryHoverButton>().description =
                                item.Value.name + "\n Quantity : " +
                                item.Value.quantity + "\n Cost : " +
                                item.Value.cost + "\n Total : " + (item.Value.cost * item.Value.quantity) +
                                (equipableText != "" ? "\n" + equipableText : "");
                                InventoryManager.instance.defense += item.Value.armorValue;
                                inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
                            });
                            break;
                        }
                    case BodyPart.LEGS:
                        {
                            rect.GetComponent<Button>().onClick.AddListener(delegate {
                                MusicManager.instance.PlayActionSound();
                                if (InventoryManager.instance.legsItem != null)
                                {
                                    InventoryManager.instance.defense -= InventoryManager.instance.legsItem.armorValue;
                                }
                                InventoryManager.instance.legsItem = item.Value; 
                                legsItemButton.transform.GetChild(1).GetComponent<Image>().sprite = item.Value.texture;
                                legsItemButton.GetComponent<InventoryHoverButton>().descriptionText = descriptionText;
                                legsItemButton.GetComponent<InventoryHoverButton>().description =
                                item.Value.name + "\n Quantity : " +
                                item.Value.quantity + "\n Cost : " +
                                item.Value.cost + "\n Total : " + (item.Value.cost * item.Value.quantity) +
                                (equipableText != "" ? "\n" + equipableText : "");
                                InventoryManager.instance.defense += item.Value.armorValue;
                                inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
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
                        InventoryManager.instance.damage -= InventoryManager.instance.weaponItem.damageValue;
                    }
                    InventoryManager.instance.weaponItem = item.Value;
                    weaponItemButton.transform.GetChild(1).GetComponent<Image>().sprite = item.Value.texture;
                    weaponItemButton.GetComponent<InventoryHoverButton>().descriptionText = descriptionText;
                    weaponItemButton.GetComponent<InventoryHoverButton>().description =
                    item.Value.name + "\n Quantity : " +
                    item.Value.quantity + "\n Cost : " +
                    item.Value.cost + "\n Total : " + (item.Value.cost * item.Value.quantity) +
                    (equipableText != "" ? "\n" + equipableText : "");
                    InventoryManager.instance.damage += item.Value.damageValue;
                    inventoryAttackCount.text = "Damage : " + InventoryManager.instance.damage;
                });
            }
        }

        inventoryDefenseCount.text = "Defense : " + InventoryManager.instance.defense;
        inventoryAttackCount.text = "Damage : " + InventoryManager.instance.damage;
        lifeSlider.value = InventoryManager.instance.life / 100;
        lifeSlider.GetComponentInChildren<Text>().text = "Life : " + InventoryManager.instance.life + " / 100";
        manaSlider.value = InventoryManager.instance.mana / 100;
        manaSlider.GetComponentInChildren<Text>().text = "Mana : " + InventoryManager.instance.mana + " / 100";
        inventoryLevelCount.text = "Level : " + InventoryManager.instance.level;
        xpSlider.value = InventoryManager.instance.xp / InventoryManager.instance.xpToNextLevel;
        xpSlider.GetComponentInChildren<Text>().text = "Exp : " + InventoryManager.instance.xp.ToString("F2") + " / " + InventoryManager.instance.xpToNextLevel;
    }

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
        //inventoryShop.GetComponent<RectTransform>().sizeDelta = new Vector2(0, items.Count * 30);
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

    public void UpdateGoldCount()
    {
        inventoryGoldShopCount.text = "Gold : " + InventoryManager.instance.currentGold.ToString("F0");
    }

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
}
