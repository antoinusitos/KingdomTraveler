using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance = null;

    [HideInInspector]
    public DialogContainer dialogContainer;

    private void Awake()
    {
        instance = this;
    }

    public void SetChoices(string[] someChoices)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < someChoices.Length; i++)
        {
            Transform temp = transform.GetChild(i);
            temp.gameObject.SetActive(true);
            temp.GetComponentInChildren<Text>().text = someChoices[i];
            if (someChoices[i] == "Leave")
            {
                temp.GetComponent<Button>().onClick.AddListener(delegate
                {
                    UIManager.instance.CloseHouse();
                    DialogManager.instance.FinishDialog();
                });
            }
            else if (someChoices[i] == "Buy")
            {
                temp.GetComponent<Button>().onClick.AddListener(delegate
                {
                    UIManager.instance.ShowShopInventory(true);
                    UIManager.instance.choices.SetActive(false);
                });
            }
        }
    }
}
