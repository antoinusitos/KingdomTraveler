using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarsData", menuName = "ScriptableObjects/StarsData", order = 1)]
public class StarsData : ScriptableObject
{
    public List<SignDefinition> starsData = new List<SignDefinition>();

    public string FindReward(SignDefinition signDefinition)
    {
        if(signDefinition.rewardType == "Object")
        {
            if(signDefinition.rewardName == "Potion")
            {
                InventoryManager.instance.AddItem(GameManager.instance.itemsData.GetGameItemWithID(6));
                return "You received a Potion";
            }
        }

        return "";
    }
}

[System.Serializable]
public class SignDefinition
{
    public List<int> starsID = new List<int>();
    public string signName = "";
    public string rewardType = "";
    public string rewardName = "";
}