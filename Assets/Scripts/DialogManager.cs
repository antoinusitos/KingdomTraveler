using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance = null;

    public DialogContainer dialogContainer;

    public GameObject blackBackground = null;
    public GameObject character = null;
    public GameObject dialog = null;
    public GameObject clickToContinue = null;

    public Text dialogText = null;
    public bool skipTextAnim = false;

    public bool allTextShown = false;
    private bool isShowningText = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.instance.dialogData.LoadDialogs();
    }

    public void SetDialogContainer(int dialogID)
    {
        dialogContainer = GameManager.instance.dialogData.GetDialogContainerWithID(dialogID);
        if (dialogContainer.dialogID != -1)
        {
            StartCoroutine(ShowText());
        }
    }

    public void ShowCharacter(bool aState)
    {
        character.SetActive(aState);
    }

    public void ShowDialog(bool aState)
    {
        dialog.SetActive(aState);
    }

    public void FinishDialog()
    {
        ShowDialog(false);
        ShowCharacter(false);
        blackBackground.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isShowningText)
        {
            skipTextAnim = true;
        }
    }

    private IEnumerator ShowText()
    {
        allTextShown = false;
        ShowCharacter(true);
        ShowDialog(true);

        isShowningText = true;
        if (dialogContainer.useBlackBackground)
            blackBackground.SetActive(true);
        else
            blackBackground.SetActive(false);

        WaitForSeconds letterTime = new WaitForSeconds(0.05f);
        Dialog storyDialog = dialogContainer.dialog;
        dialogText.text = "";
        for (int l = 0; l < storyDialog.dialog.Length; l++)
        {
            if (skipTextAnim)
            {
                skipTextAnim = false;
                for (int l2 = l; l2 < storyDialog.dialog.Length; l2++)
                {
                    dialogText.text += storyDialog.dialog[l2];
                }
                break;
            }
            dialogText.text += storyDialog.dialog[l];
            yield return letterTime;
        }
        if (!storyDialog.autoPass)
        {
            clickToContinue.SetActive(true);
            InputManager.instance.SetWaitingForInput();
            while (!InputManager.instance.GetInputDone())
            {
                yield return null;
            }
        }
        skipTextAnim = false;
        if (dialogContainer.dialog.choices.Count > 0)
        {
            UIManager.instance.choices.SetActive(true);
            ChoiceManager.instance.SetChoices(dialogContainer.dialog.choices.ToArray());
            ChoiceManager.instance.dialogContainer = dialogContainer;
        }
        isShowningText = false;
        skipTextAnim = false;
        allTextShown = true;
    }
}
