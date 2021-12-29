using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestsData", menuName = "ScriptableObjects/QuestsData", order = 1)]
public class QuestsData : ScriptableObject
{
    public List<Quest> questsData = new List<Quest>();

    public Quest GetQuestWithID(int anID)
    {
        for (int i = 0; i < questsData.Count; i++)
        {
            if (questsData[i].questID == anID)
                return questsData[i];
        }
        return null;
    }
}