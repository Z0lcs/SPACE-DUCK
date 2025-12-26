using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();

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

    private void Update()
    {
        if (QuestInputTracker.Instance != null)
        {
            foreach (QuestSO quest in QuestInputTracker.Instance.activeQuests)
            {
                foreach (QuestObjective objective in quest.questObjectives)
                {
                    if (objective.targetLocation != null || objective.targetItem != null)
                    {
                        UpdateObjectiveProgress(quest, objective);
                    }
                }
            }
        }
    }

    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (questSO == null || objective == null) return;

        if (!questProgress.ContainsKey(questSO))
        {
            questProgress[questSO] = new Dictionary<QuestObjective, int>();
        }

        var progressDict = questProgress[questSO];

        if (!progressDict.ContainsKey(objective))
        {
            progressDict[objective] = 0;
        }

        if (objective.targetLocation != null && GameManager.Instance.LocationTrack.HasVisited(objective.targetLocation))
        {
            if (progressDict[objective] != objective.requiredAmount)
            {
                progressDict[objective] = objective.requiredAmount;
                Debug.Log($"<color=green>[QUEST]</color> Helyszín elérve: {objective.targetLocation.displayName}");
            }
        }
        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
        {
            if (progressDict[objective] != objective.requiredAmount)
            {
                progressDict[objective] = objective.requiredAmount;
                Debug.Log($"<color=green>[QUEST]</color> Beszéltél vele: {objective.targetNPC.name}");
            }
        }
        else if (objective.targetKey != null)
        {
            if (progressDict[objective] < objective.requiredAmount && objective.targetKey.IsAnyKeyPressed())
            {
                progressDict[objective] += 1;
                Debug.Log($"<color=cyan>[QUEST]</color> {questSO.questName} haladás: {progressDict[objective]}/{objective.requiredAmount}");
            }
        }
        else if (objective.targetItem != null)
        {
            int currentCount = Inventory.Instance.GetItemQuantity(objective.targetItem);

            if (progressDict[objective] != currentCount)
            {
                progressDict[objective] = Mathf.Min(currentCount, objective.requiredAmount);
                Debug.Log($"<color=orange>[QUEST]</color> {objective.targetItem.itemName} mennyiség: {progressDict[objective]}/{objective.requiredAmount}");
            }
        }
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