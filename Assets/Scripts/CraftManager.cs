using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    public static CraftManager instance = null;

    public ItemsData itemsData = null;

    public int itemIDWantedToCraft = -1;

    public Text itemName = null;
    public Text itemDesc = null;
    public Text needName = null;
    public Transform itemRequirement = null;
    public Button craftButton = null;

    private GameItem currentItem = null;
    private bool canCraft = false;

    public Button itemsButtonPrefab = null;
    public Transform itemsButtonPanel = null;
    public Image needPrefab = null;

    private bool needUpdate = true;

    private bool DEBUGCRAFT = true;

    private void Awake()
    {
        instance = this;
    }

    public void ToggleNeedUpdate()
    {
        needUpdate = true;
    }

    public void UpdateCraftList()
    {
        if (!needUpdate)
            return;

        needUpdate = false;

        if (itemsButtonPanel.childCount > 0)
        {
            for (int c = 0; c < itemsButtonPanel.childCount; c++)
            {
                Destroy(itemsButtonPanel.GetChild(0).gameObject);
            }
        }

        if (DEBUGCRAFT)
        {
            for (int i = 0; i < itemsData.itemsData.Count; i++)
            {
                GameItem item = itemsData.GetGameItemWithID(itemsData.itemsData[i].ID);
                if (item != null)
                {
                    Button b = Instantiate(itemsButtonPrefab, itemsButtonPanel);
                    b.transform.GetChild(0).GetComponent<Image>().sprite = item.texture;
                    b.onClick.AddListener(delegate { WantToCraft(item.ID); });
                }
            }
        }
        else
        {
            for (int i = 0; i < itemsData.craftData.Count; i++)
            {
                GameItem item = itemsData.GetGameItemWithID(itemsData.craftData[i]);
                if (item != null)
                {
                    Button b = Instantiate(itemsButtonPrefab, itemsButtonPanel);
                    b.transform.GetChild(0).GetComponent<Image>().sprite = item.texture;
                    b.onClick.AddListener(delegate { WantToCraft(item.ID); });
                }
            }
        }
    }

    public void WantToCraft(int anID)
    {
        itemIDWantedToCraft = anID;
        canCraft = true;
        currentItem = itemsData.GetGameItemWithID(itemIDWantedToCraft);
        if (currentItem != null)
        {
            itemName.text = currentItem.name;
            itemDesc.text = "Description of " + currentItem.name;
                
            if(itemRequirement.childCount > 0)
            {
                for(int c = 0; c < itemRequirement.childCount; c++)
                {
                    Destroy(itemRequirement.GetChild(0).gameObject);
                }
            }

            for(int j = 0; j < currentItem.requirements.Length; j++)
            {
                for (int k = 0; k < itemsData.itemsData.Count; k++)
                {
                    if (itemsData.itemsData[k].ID == currentItem.requirements[j].ID)
                    {
                        Image b = Instantiate(needPrefab, itemRequirement);
                        b.sprite = itemsData.itemsData[k].texture;
                        InventoryHoverButton inventoryHoverButton = b.GetComponent<InventoryHoverButton>();
                        inventoryHoverButton.description = itemsData.GetGameItemWithID(currentItem.requirements[j].ID).name;
                        inventoryHoverButton.descriptionText = needName;
                        int quantity = InventoryManager.instance.GetItemQuantity(itemsData.itemsData[k].ID);
                        b.GetComponentInChildren<Text>().text = quantity + " / " + currentItem.requirements[j].quantity;
                        if (currentItem.requirements[j].quantity > quantity)
                        {
                            b.color = Color.red;
                            canCraft = false;
                        }
                        break;
                    }
                }
            }
                
            if(canCraft)
            {
                craftButton.interactable = true;
            }
            else
            {
                craftButton.interactable = false;
            }
        }
        else
        {
            canCraft = false;
            craftButton.interactable = false;
        }
    }

    public void Craft()
    {
        if(currentItem != null && canCraft)
        {
            if(currentItem.requirements.Length > 0)
            {
                for(int r = 0; r < currentItem.requirements.Length; r++)
                {
                    if (InventoryManager.instance.GetItemQuantity(currentItem.requirements[r].ID) < currentItem.requirements[r].quantity)
                    {
                        return;
                    }
                }
                GameItem item = GameManager.instance.GetGameItemWithID(itemIDWantedToCraft);
                if (item != null)
                {
                    InventoryManager.instance.AddItem(item);
                    for (int r = 0; r < currentItem.requirements.Length; r++)
                    {
                        InventoryManager.instance.RemoveItem(currentItem.requirements[r].ID, currentItem.requirements[r].quantity);
                    }
                }
            }
            else
            {
                GameItem item = GameManager.instance.GetGameItemWithID(itemIDWantedToCraft);
                if (item != null)
                {
                    InventoryManager.instance.AddItem(item);
                }
            }
            WantToCraft(currentItem.ID);
            return;
        }
    }
}
