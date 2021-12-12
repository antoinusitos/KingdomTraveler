using UnityEngine;

public class Location : MonoBehaviour
{
    public int townID = -1;

    public Node point = null;

    public TextMesh townNameText = null;

    private TownContainer townContainer;

    public bool isReachable = false;

    private void Start()
    {
        townContainer = GameManager.instance.townData.GetTownContainerWithID(townID);
        townNameText.text = townContainer.townName;
    }

    private void Update()
    {
        
    }
}
