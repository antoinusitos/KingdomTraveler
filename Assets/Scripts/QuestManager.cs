using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestType
{
    MAIN,
    SIDE
}

public enum QuestStepRequirement
{
    NONE,
    AI,
    GOLD,
    CITY
}

[System.Serializable]
public struct QuestStep
{
    public string stepObjective;
    public bool stepDone;
    public QuestStepRequirement questStepRequirement;
    public string stepDetail;
}

[System.Serializable]
public class Quest
{
    public int questID = -1;
    public QuestType questType = QuestType.MAIN;
    public string questTitle = "";
    public string questDescription = "";
    public bool questFinished = false;
    public QuestStep[] questSteps = null;
    private int stepIndex = 0;

    public Quest()
    {

    }

    public Quest(Quest toCopy)
    {
        questID = toCopy.questID;
        questType = toCopy.questType;
        questTitle = toCopy.questTitle;
        questDescription = toCopy.questDescription;
        questFinished = toCopy.questFinished;
        questSteps = new QuestStep[toCopy.questSteps.Length];
        for (int i = 0; i < toCopy.questSteps.Length; i++)
        {
            questSteps[i].stepDetail = toCopy.questSteps[i].stepDetail;
            questSteps[i].questStepRequirement = toCopy.questSteps[i].questStepRequirement;
            questSteps[i].stepDone = toCopy.questSteps[i].stepDone;
            questSteps[i].stepObjective = toCopy.questSteps[i].stepObjective;
        }
        stepIndex = toCopy.stepIndex;
    }

    public void CompleteStep()
    {
        questSteps[stepIndex].stepDone = true;
        stepIndex++;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance = null;

    public List<Quest> activeQuests = new List<Quest>();

    public Text questText = null;
    public Transform questButtonsPanel = null;

    public Button questButtonPrefab = null;

    public Text questSeparationText = null;

    public Text questLaunchedText = null;

    private List<Quest> finishedQuests = new List<Quest>();
    private List<Quest> startedQuests = new List<Quest>();

    private bool showing = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(showing)
        {
            return;
        }

        if (finishedQuests.Count > 0)
        {
            showing = true;
            StartCoroutine(QuestTriggeredTimer("Finished : " + finishedQuests[0].questTitle));
            finishedQuests.RemoveAt(0);
        }
        else if (startedQuests.Count > 0)
        {
            showing = true;
            StartCoroutine(QuestTriggeredTimer("Started : " + startedQuests[0].questTitle));
            startedQuests.RemoveAt(0);
        }
    }

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        startedQuests.Add(quest);
    }

    public void RefreshQuests()
    {
        for(int i = 0; i < questButtonsPanel.childCount; i++)
        {
            Destroy(questButtonsPanel.GetChild(i).gameObject);
        }

        Instantiate(questSeparationText, questButtonsPanel);
        int questsAdded = 0;
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questType != QuestType.MAIN)
                continue;
            questsAdded++;
            Button button = Instantiate(questButtonPrefab, questButtonsPanel);
            Quest localQuest = new Quest(activeQuests[i]);
            button.GetComponentInChildren<Text>().text = localQuest.questTitle;
            button.onClick.AddListener(delegate {
                SelectQuest(localQuest);
            });
        }
        if (questsAdded >= activeQuests.Count)
            return;

        Text secondary = Instantiate(questSeparationText, questButtonsPanel);
        secondary.text = "Side Quests";
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questType != QuestType.SIDE)
                continue;
            Button button = Instantiate(questButtonPrefab, questButtonsPanel);
            Quest localQuest = activeQuests[i];
            button.GetComponentInChildren<Text>().text = localQuest.questTitle;
            button.onClick.AddListener(delegate {
                SelectQuest(localQuest);
            });
        }
    }

    public void SelectQuest(Quest quest)
    {
        questText.text = "<size=20><b>" + quest.questTitle + "</b></size>\n\n" + quest.questDescription + "\n";
        for(int i = 0; i < quest.questSteps.Length; i++)
        {
            questText.text += "\t- " + quest.questSteps[i].stepObjective + " = " + (quest.questSteps[i].stepDone ? "Done" : "To Do") + "\n";
        }
    }

    private IEnumerator QuestTriggeredTimer(string questTitle)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);
        questLaunchedText.text = "";
        for (int l = 0; l < questTitle.Length; l++)
        {
            questLaunchedText.text += questTitle[l];
            yield return waitForSeconds;
        }
        yield return new WaitForSeconds(2);
        questLaunchedText.text = "";
        showing = false;
    }

    public void CheckPlaceVisited(int id)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            for (int s = 0; s < activeQuests[i].questSteps.Length; s++)
            {
                if (activeQuests[i].questSteps[s].stepDone) continue;

                if (activeQuests[i].questSteps[s].questStepRequirement == QuestStepRequirement.CITY &&
                    int.Parse(activeQuests[i].questSteps[s].stepDetail) == id)
                {
                    activeQuests[i].questSteps[s].stepDone = true;
                    if(CheckQuestFinished(activeQuests[i]))
                    {
                        break;
                    }
                }
            }
        }
    }

    public void CheckAIVisited(int id)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            for (int s = 0; s < activeQuests[i].questSteps.Length; s++)
            {
                if (activeQuests[i].questSteps[s].stepDone) continue;

                if(activeQuests[i].questSteps[s].questStepRequirement == QuestStepRequirement.AI &&
                    int.Parse(activeQuests[i].questSteps[s].stepDetail) == id)
                {
                    activeQuests[i].questSteps[s].stepDone = true;
                    if (CheckQuestFinished(activeQuests[i]))
                    {
                        break;
                    }
                }
            }
        }
    }

    private bool CheckQuestFinished(Quest quest)
    {
        for (int s = 0; s < quest.questSteps.Length; s++)
        {
            if (!quest.questSteps[s].stepDone) 
                return false;
        }
        FinishedQuest(quest);
        return true;
    }

    public void FinishedQuest(Quest quest)
    {
        finishedQuests.Add(quest);
        activeQuests.Remove(quest);
    }
}
