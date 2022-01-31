using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int aiID = -1;
    public AIInteration[] aIInterations = null;
    public List<InventoryItem> inventory;
    public bool finishedInteraction = false;

    public void Interact()
    {
        finishedInteraction = false;
        UIManager.instance.FillShopInventory(inventory);
        StartCoroutine(Interation());
    }

    private IEnumerator Interation()
    {
        for(int i = 0; i < aIInterations.Length; i++)
        {
            if(aIInterations[i].command == "Dialog")
            {
                DialogManager.instance.SetDialogContainer(aIInterations[i].argument);
                while(!DialogManager.instance.allTextShown)
                {
                    yield return null;
                }
                //DialogManager.instance.FinishDialog();
            }
            else if (aIInterations[i].command == "AddGold")
            {
                InventoryManager.instance.AddGold(aIInterations[i].argument);
                while (!InventoryManager.instance.GetItemJustAdded())
                {
                    yield return null;
                }
            }
            else if (aIInterations[i].command == "AddObject")
            {

            }
            else if (aIInterations[i].command == "AddQuest")
            {
                QuestManager.instance.AddQuest(new Quest(GameManager.instance.questsData.GetQuestWithID(aIInterations[i].argument)));
            }
            else if (aIInterations[i].command == "RemoveGold")
            {

            }
            else if (aIInterations[i].command == "RemoveObject")
            {

            }
            else if (aIInterations[i].command == "Close")
            {
                DialogManager.instance.FinishDialog();
            }
        }
        finishedInteraction = true;
        QuestManager.instance.CheckAIVisited(aiID);
    }
}
