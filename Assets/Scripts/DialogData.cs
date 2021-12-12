using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "ScriptableObjects/DialogObject", order = 1)]
public class DialogData : ScriptableObject
{
    public List<DialogContainer> dialogData = new List<DialogContainer>();

    public DialogContainer GetDialogContainerWithID(int anID)
    {
        for(int i = 0; i < dialogData.Count; i++)
        {
            if (dialogData[i].dialogID == anID)
                return dialogData[i];
        }
        return new DialogContainer();
    }
}

[System.Serializable]
public struct DialogContainer
{
    public int dialogID;
    public List<Dialog> dialog;
    public List<InventoryItem> inventory;

    public DialogContainer(int anID = -1)
    {
        dialogID = anID;
        dialog = new List<Dialog>();
        inventory = new List<InventoryItem>();
    }
}

[System.Serializable]
public struct Dialog
{
    public string dialog;
    public string[] choices;

    public Dialog(string aDialog = "")
    {
        dialog = aDialog;
        choices = null;
    }
}