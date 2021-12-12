using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class House : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private string houseName = "";

    [SerializeField]
    private int houseID = -1;
    [SerializeField]

    private int dialogID = -1;

    private Image image = null;

    [SerializeField]
    private GameObject houseBackground = null;

    DialogContainer dialogContainer;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.ShowCharacter(true);
        UIManager.instance.ShowTownDialogGameObject(true);
        if(dialogID != -1)
        {
            dialogContainer = GameManager.instance.dialogData.GetDialogContainerWithID(dialogID);
            if (dialogContainer.dialog != null && dialogContainer.dialog.Count > 0)
            {
                UIManager.instance.SetTownDialogText(dialogContainer.dialog[0].dialog);
                UIManager.instance.choices.SetActive(true);
                ChoiceManager.instance.SetChoices(dialogContainer.dialog[0].choices);
            }
            ChoiceManager.instance.dialogContainer = dialogContainer;
            UIManager.instance.FillShopInventory(dialogContainer.inventory);
        }
        houseBackground.SetActive(true);
        UIManager.instance.currentHouse = this;
    }

    public void Close()
    {
        UIManager.instance.ShowCharacter(false);
        UIManager.instance.ShowTownDialogGameObject(false);
        UIManager.instance.choices.SetActive(false);
        houseBackground.SetActive(false);
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

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        
    }
}
