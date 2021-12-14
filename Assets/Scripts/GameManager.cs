using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PageType
{
    MAP,
    INVENTORY,
    STARS,
    CRAFT,
    TOWN
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public PageType currentPageType = PageType.MAP;
    public PageType lastPageType = PageType.MAP;

    public GameObject uiManager = null;

    public bool isInTown = true;
    public bool isInMenu = false;
    public bool isMoving = false;

    private LineRenderer lineRenderer = null;

    public TownData townData = null;
    public DialogData dialogData = null;
    public ItemsData itemsData = null;

    public bool gameWaiting = true;

    public bool skipIntro = false;

    public Village cameraManager = null;

    public GameObject storyGameObject = null;
    public Text storyText = null;
    private string[] storyTextInput = new string[] {
    "Welcome young master and sorry for the mess in the castle.",
    "As you can see your parent's castle has been totally destroyed by one of our enemy.",
    "Your people ran away and are now dispatched everywhere.",
    "I've marked on your map some locations around the castle to start your investigations.",
    "And please find something more serious to wear, people won't follow you and you won't last long wearing that...",
    "Good luck !",
    };

    public GameObject console = null;

    private void Awake()
    {
        instance = this;
        uiManager.SetActive(true);
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if(skipIntro)
        {
            StartGame();
            return;
        }
        StartCoroutine(StartIntro());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            console.SetActive(!console.activeSelf);
        }
    }

    private IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(2);
        storyGameObject.SetActive(true);
        storyText.text = "";
        WaitForSeconds letterTime = new WaitForSeconds(0.05f);
        for (int i = 0; i < storyTextInput.Length; i++)
        {
            for (int l = 0; l < storyTextInput[i].Length; l++)
            {
                storyText.text += storyTextInput[i][l];
                yield return letterTime;
            }
            yield return new WaitForSeconds(2);
            storyText.text = "";
        }
        InventoryManager.instance.AddGold(100);
        storyGameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        cameraManager.StartGame();
    }

    public void ShowRenderer(List<Node> nodes)
    {
        lineRenderer.positionCount = nodes.Count;
        int index = 0;
        for(int i = nodes.Count - 1; i >= 0 ; i--)
        {
            lineRenderer.SetPosition(index, nodes[i].transform.position);
            index++;
        }
    }

    public void RemoveOneLine()
    {
        lineRenderer.positionCount--;
    }

    public void SetCurrentLinePos(Vector3 pos)
    {
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public GameItem GetGameItemWithID(int anID)
    {
        for(int i = 0; i < itemsData.itemsData.Count; i++)
        {
            if(itemsData.itemsData[i].ID == anID)
            {
                return itemsData.itemsData[i];
            }
        }

        return null;
    }
}
