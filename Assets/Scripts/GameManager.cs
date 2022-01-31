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
    TOWN,
    QUEST
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
    public DialogDataBank dialogData = null;
    public ItemsData itemsData = null;
    public QuestsData questsData = null;

    public bool gameWaiting = true;

    public bool skipIntro = false;

    public Village cameraManager = null;

    public Text storyText = null;

    public GameObject console = null;

    private AI gameStoryAI = null;

    private void Awake()
    {
        instance = this;
        uiManager.SetActive(true);
        lineRenderer = GetComponent<LineRenderer>();
        gameStoryAI = GetComponent<AI>();
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
        gameStoryAI.Interact();
        while (!gameStoryAI.finishedInteraction)
            yield return null;
        DialogManager.instance.FinishDialog();
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
