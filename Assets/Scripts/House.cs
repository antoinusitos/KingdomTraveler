using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum HouseType
{
    NONE,
    TAVERN,
    SHOP
}

public class House : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private string houseName = "";

    public int houseID = -1;

    private Image image = null;

    [SerializeField]
    private GameObject houseBackground = null;

    private bool isActive = false;

    public HouseType houseType = HouseType.NONE;

    private AI localAI = null;


    private void Start()
    {
        image = GetComponent<Image>();
        localAI = GetComponentInChildren<AI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AI interaction = GetComponent<AI>();
        if(interaction != null)
        {
            interaction.Interact();
        }
        houseBackground.SetActive(true);
        UIManager.instance.currentHouse = this;
        isActive = true;
    }

    public void Close()
    {
        DialogManager.instance.FinishDialog();
        UIManager.instance.choices.SetActive(false);
        houseBackground.SetActive(false);
        isActive = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetHouseName(houseName);
        image.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetHouseName("");
        image.color = Color.white;
    }

    private void Update()
    {
        if(isActive)
        {
            switch(houseType)
            {
                case HouseType.TAVERN:
                    if (DialogManager.instance.allTextShown)
                    {
                        if(localAI != null && localAI.finishedInteraction)
                        {
                            QuestManager.instance.CheckAIVisited(localAI.aiID);
                            Close();
                        }
                        else if (localAI == null)
                        {
                            Close();
                        }
                    }
                    break;
            }
        }
    }
}
