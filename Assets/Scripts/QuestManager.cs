using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestType
{
    MAIN,
    SIDE
}

[System.Serializable]
public struct QuestStep
{
    public string stepObjective;
    public bool stepDone;
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

    private void Awake()
    {
        instance = this;
    }

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        StartCoroutine(QuestTriggeredTimer("Started : " + quest.questTitle));
    }

    public void RefreshQuests()
    {
        Instantiate(questSeparationText, questButtonsPanel);
        int questsAdded = 0;
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questType != QuestType.MAIN)
                continue;
            questsAdded++;
            Button button = Instantiate(questButtonPrefab, questButtonsPanel);
            Quest localQuest = activeQuests[i];
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
    }
}
