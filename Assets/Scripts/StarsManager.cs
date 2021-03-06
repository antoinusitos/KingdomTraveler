using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsManager : MonoBehaviour
{
    public static StarsManager instance = null;

    public List<Star> starsSelected = new List<Star>();
    public List<int> starsIDSelected = new List<int>();

    public StarsData starsData = null;

    public Transform starsPanel = null;
    private RectTransform starsPanelTransform = null;

    public Text constellationFoundText = null;
    public Text constellationRewardText = null;

    private Vector2 movePos = Vector2.zero;
    private float cameraMoveSpeed = 20;

    private const float rightLimit = 2;
    private const float leftLimit = 368;
    private const float topLimit = -88;
    private const float bottomLimit = 88;

    private float lastPosX = rightLimit;

    public Image starClue = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        starsPanelTransform = starsPanel.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && GameManager.instance.currentPageType == PageType.STARS)
        {
            movePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1) && GameManager.instance.currentPageType == PageType.STARS)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            starsPanelTransform.anchoredPosition -= (movePos - mousePos) * Time.deltaTime * cameraMoveSpeed;
            if(starsPanelTransform.anchoredPosition.x < rightLimit)
            {
                starsPanelTransform.anchoredPosition = new Vector2(rightLimit, starsPanelTransform.anchoredPosition.y);
            }
            if (starsPanelTransform.anchoredPosition.x > leftLimit)
            {
                starsPanelTransform.anchoredPosition = new Vector2(leftLimit, starsPanelTransform.anchoredPosition.y);
            }
            if (starsPanelTransform.anchoredPosition.y < topLimit)
            {
                starsPanelTransform.anchoredPosition = new Vector2(starsPanelTransform.anchoredPosition.x, topLimit);
            }
            if (starsPanelTransform.anchoredPosition.y > bottomLimit)
            {
                starsPanelTransform.anchoredPosition = new Vector2(starsPanelTransform.anchoredPosition.x, bottomLimit);
            }
            lastPosX = starsPanelTransform.anchoredPosition.x;
            movePos = Input.mousePosition;
        }
    }

    public void MovePanelOutsideOfView()
    {
        starsPanelTransform.anchoredPosition = new Vector2(rightLimit, starsPanelTransform.anchoredPosition.y);
    }

    public void RevertToLastPosX()
    {
        starsPanelTransform.anchoredPosition = new Vector2(lastPosX, starsPanelTransform.anchoredPosition.y);
    }

    public void AddSelectedStar(Star aStar)
    {
        if(!starsSelected.Contains(aStar))
        {
            starsSelected.Add(aStar);
            starsIDSelected.Add(aStar.ID);
            aStar.imageLinked.color = Color.red;
        }
        else
        {
            starsSelected.Remove(aStar);
            starsIDSelected.Remove(aStar.ID);
            aStar.imageLinked.color = Color.white;
        }
       CheckStarsSelected();
    }

    public void CheckStarsSelected()
    {
        for(int i = 0; i < starsData.starsData.Count; i++)
        {
            if (starsSelected.Count == starsData.starsData[i].starsID.Count)
            {
                bool found = true;
                for (int s = 0; s < starsData.starsData[i].starsID.Count; s++)
                {
                    if (!starsIDSelected.Contains(starsData.starsData[i].starsID[s]))
                    {
                        found = false;
                        break;
                    }
                }
                if(found)
                {
                    for (int s = 0; s < starsData.starsData[i].starsID.Count; s++)
                    {
                        float ax = 0;
                        float ay = 0;
                        float bx = 0;
                        float by = 0;
                        for (int j = 0; j < starsSelected.Count; j++)
                        {
                            //last line
                            if(s == starsData.starsData[i].starsID.Count - 1)
                            {
                                if (starsSelected[j].ID == starsData.starsData[i].starsID[0])
                                {
                                    ax = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.x;
                                    ay = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.y;
                                }
                                else if (starsSelected[j].ID == starsData.starsData[i].starsID[starsData.starsData[i].starsID.Count - 1])
                                {
                                    bx = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.x;
                                    by = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.y;
                                }
                            }
                            else
                            {
                                if (starsSelected[j].ID == starsData.starsData[i].starsID[s])
                                {
                                    ax = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.x;
                                    ay = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.y;
                                }
                                else if (starsSelected[j].ID == starsData.starsData[i].starsID[s + 1])
                                {
                                    bx = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.x;
                                    by = starsSelected[j].GetComponent<RectTransform>().anchoredPosition.y;
                                }
                            }
                        }
                        MakeLine(ax, ay, bx, by, Color.red);
                    }

                    for (int j = 0; j < starsSelected.Count; j++)
                    {
                        starsSelected[j].GetComponent<Button>().interactable = false;
                    }
                    starsSelected.Clear();
                    constellationFoundText.text = "Constellation " + starsData.starsData[i].signName + " Found !";
                    constellationFoundText.gameObject.SetActive(true);
                    string reward = starsData.FindReward(starsData.starsData[i]);
                    constellationRewardText.text = reward;
                    constellationRewardText.gameObject.SetActive(true);
                    StartCoroutine(CloseFoundText());
                    return;
                }
            }
        }
    }

    private IEnumerator CloseFoundText()
    {
        yield return new WaitForSeconds(4);
        constellationFoundText.gameObject.SetActive(false);
        constellationRewardText.gameObject.SetActive(false);
    }

    void MakeLine(float ax, float ay, float bx, float by, Color col)
    {
        GameObject NewObj = new GameObject("StarsLink");
        Image NewImage = NewObj.AddComponent<Image>();
        NewImage.color = col;
        RectTransform rect = NewObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.SetParent(starsPanel);
        rect.localScale = Vector3.one;

        Vector3 a = new Vector3(ax, ay, 0);
        Vector3 b = new Vector3(bx, by, 0);

        rect.anchoredPosition = new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, 0);
        Vector3 dif = a - b;
        rect.sizeDelta = new Vector3(dif.magnitude, 0.3f);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
    }
}
