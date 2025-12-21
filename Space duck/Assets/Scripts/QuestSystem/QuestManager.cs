using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();

    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSO))
        {
            questProgress[questSO] = new Dictionary<QuestObjective, int>();
        }

        var progressDict = questProgress[questSO];

        if (!progressDict.ContainsKey(objective))
        {
            progressDict[objective] = 0;
        }

        if (objective.targetItem != null)
        {
            // Példa: progressDict[objective] = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
        }

        else if (objective.targetLocation != null && GameManager.Instance.LocationTrack.HasVisited(objective.targetLocation))
        {
            progressDict[objective] = objective.requiredAmount;
        }

        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
        {
            progressDict[objective] = objective.requiredAmount;
        }

        else if (objective.targetKey != null)
        {
            if (progressDict[objective] < objective.requiredAmount && objective.targetKey.IsAnyKeyPressed())
            {
                progressDict[objective] += 1;
            }
        }
    }

    public string GetProgressText(QuestSO questSO, QuestObjective objective)
    {
        int currentAmount = GetCurrentAmount(questSO, objective);

        if (currentAmount >= objective.requiredAmount)
            return "Complete ";

        if (objective.targetItem != null || objective.targetKey != null)
            return $"{currentAmount}/{objective.requiredAmount}";

        return "In progress";
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