using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogDataBank", menuName = "ScriptableObjects/DialogBankObject", order = 1)]
public class DialogDataBank : ScriptableObject
{
    public DialogData dialogData;

    public DialogContainer GetDialogContainerWithID(int anID)
    {
        for(int i = 0; i < dialogData.dialogData.Count; i++)
        {
            if (dialogData.dialogData[i].dialogID == anID)
                return dialogData.dialogData[i];
        }
        return new DialogContainer();
    }

    public void LoadDialogs()
    {
        string path = Application.dataPath + "/Data/Dialog.json";
        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            dialogData = JsonUtility.FromJson<DialogData>(json);
        }
    }
}

[System.Serializable]
public class DialogData
{
    public List<DialogContainer> dialogData;
}

[System.Serializable]
public class DialogContainer
{
    public int dialogID;
    public Dialog dialog;
    public bool useBlackBackground;

    public DialogContainer(int anID = -1)
    {
        dialogID = anID;
        dialog = null;
        useBlackBackground = false;
    }
}

[System.Serializable]
public class Dialog
{
    public string dialog;
    public List<string> choices;
    public bool autoPass;

    public Dialog(string aDialog = "")
    {
        dialog = aDialog;
        choices = null;
        autoPass = false;
    }
}