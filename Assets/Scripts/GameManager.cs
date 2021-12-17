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

    public GameObject console = null;

    private bool ReadyToContinue = true;
    public GameObject clickToContinue = null;
	private int gameStoryDialogID = 1;

    private bool skipTextAnim = false;
    private bool isInIntro = false;

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

        if(Input.GetMouseButtonDown(0) && isInIntro)
        {
            skipTextAnim = true;
        }
    }

    private IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(2);
        storyGameObject.SetActive(true);
        storyText.text = "";
        WaitForSeconds letterTime = new WaitForSeconds(0.05f);
        DialogContainer gameStoryDialogContainer = dialogData.GetDialogContainerWithID(gameStoryDialogID);
        Dialog[] storyDialog = gameStoryDialogContainer.dialog.ToArray();
        for (int i = 0; i < storyDialog.Length; i++)
        {
            isInIntro = true;
            for (int l = 0; l < storyDialog[i].dialog.Length; l++)
            {
                if(skipTextAnim)
                {
                    skipTextAnim = false;
                    for (int l2 = l; l2 < storyDialog[i].dialog.Length; l2++)
                    {
                        storyText.text += storyDialog[i].dialog[l2];
                    }
                    break;
                }
                storyText.text += storyDialog[i].dialog[l];
                yield return letterTime;
            }
            isInIntro = false;
            yield return new WaitForSeconds(0.2f);
            ReadyToContinue = false;
            clickToContinue.SetActive(true);
            while (!ReadyToContinue)
            {
                yield return null;
            }
            storyText.text = "";
        }
        InventoryManager.instance.AddGold(100);
        storyGameObject.SetActive(false);
        StartGame();
    }

    public void SetReadyToContinue()
    {
        ReadyToContinue = true;
        clickToContinue.SetActive(false);
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
