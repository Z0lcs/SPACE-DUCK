using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<QuestSO> activeQuests = new List<QuestSO>();
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private HashSet<QuestSO> completedQuests = new();
    public static event Action<QuestSO> OnQuestCompleted;
    [Header("Questek")]
    public List<QuestSO> allAvailableQuests; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this); 
            return;
        }
    }
    private void Start()
    {
        for (int i = 0; i < allAvailableQuests.Count; i++)
        {
            if (i < 5) 
            {
                AcceptQuest(allAvailableQuests[i]);
            }
        }
    }
    public void AcceptQuest(QuestSO newQuest)
    {
        if (newQuest != null && !activeQuests.Contains(newQuest) && !completedQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
        }
    }
    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (questSO == null || objective == null) return;
        if (completedQuests.Contains(questSO)) return;

        if (!questProgress.ContainsKey(questSO))
            questProgress[questSO] = new Dictionary<QuestObjective, int>();

        var progressDict = questProgress[questSO];

        if (!progressDict.ContainsKey(objective))
            progressDict[objective] = 0;

        int oldProgress = progressDict[objective];

        if (objective.targetLocation != null)
        {
            if (GameManager.Instance.LocationTrack.HasVisited(objective.targetLocation))
                progressDict[objective] = objective.requiredAmount;
        }
        else if (objective.targetNPC != null)
        {
            if (GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
                progressDict[objective] = objective.requiredAmount;
        }
        else if (objective.targetKey != null)
        {
            if (progressDict[objective] < objective.requiredAmount && objective.targetKey.IsAnyKeyPressed())
                progressDict[objective] += 1;
        }
        else if (objective.targetItem != null)
        {
            int currentCount = Inventory.Instance.GetItemQuantity(objective.targetItem);
            progressDict[objective] = Mathf.Min(currentCount, objective.requiredAmount);
        }

        if (progressDict[objective] != oldProgress)
        {
            string status = (progressDict[objective] >= objective.requiredAmount) ? "TELJESÍTVE" : "HALADÁS";
            Debug.Log($"<color=yellow>[QUEST UPDATE]</color> {questSO.questName} -> {objective.description}: " +
                      $"{progressDict[objective]}/{objective.requiredAmount} ({status})");

            CheckIfQuestIsReady(questSO);
        }
    }
    private void CheckIfQuestIsReady(QuestSO questSO)
    {
        bool allObjectivesDone = true;

        foreach (var objective in questSO.questObjectives)
        {
            if (GetCurrentAmount(questSO, objective) < objective.requiredAmount)
            {
                allObjectivesDone = false;
                break;
            }
        }

        if (allObjectivesDone)
        {
            Debug.Log("Minden cél kész, hívom a CompleteQuest-et!"); 
            CompleteQuest(questSO);
        }
    }
    public void CompleteQuest(QuestSO questSO)
    {
        if (questSO == null || completedQuests.Contains(questSO)) return;

        activeQuests.Remove(questSO);
        completedQuests.Add(questSO);


        if (questProgress.ContainsKey(questSO))
        {
            questProgress.Remove(questSO);
        }
        foreach (QuestSO q in allAvailableQuests)
        {
            if (!activeQuests.Contains(q) && !completedQuests.Contains(q))
            {
                if (activeQuests.Count < 5) 
                {
                    AcceptQuest(q);
                    break; 
                }
            }
        }

        OnQuestCompleted?.Invoke(questSO);
    }
    public bool IsQuestCompleted(QuestSO questSO)
    {
        return completedQuests.Contains(questSO);
    }
    public string GetProgressText(QuestSO questSO, QuestObjective objective)
    {
        int currentAmount = GetCurrentAmount(questSO, objective);

        if (currentAmount >= objective.requiredAmount)
            return "Kész";

        if (objective.targetItem != null || objective.targetKey != null)
            return $"{currentAmount}/{objective.requiredAmount}";

        return "Folyamatban";
    }

    public int GetCurrentAmount(QuestSO questSO, QuestObjective objective)
    {
        if (questProgress.TryGetValue(questSO, out var objectiveDictionary))
        {
            if (objectiveDictionary.TryGetValue(objective, out int amount))
            {
                return amount;
            }
        }
        return 0;
    }
}