using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TownData", menuName = "ScriptableObjects/DataObjects", order = 1)]
public class TownData : ScriptableObject
{
    public List<TownContainer> townsData = new List<TownContainer>();

    public TownContainer GetTownContainerWithID(int anID)
    {
        for(int i = 0; i < townsData.Count; i++)
        {
            if (townsData[i].townID == anID)
                return townsData[i];
        }
        return new TownContainer();
    }
}

[System.Serializable]
public struct TownContainer
{
    public int townID;
    public string townName;
    public int townLevel;
    public int townType;

    public TownContainer(int anID = -1)
    {
        townID = anID;
        townName = "";
        townLevel = -1;
        townType = 0;
    }
}