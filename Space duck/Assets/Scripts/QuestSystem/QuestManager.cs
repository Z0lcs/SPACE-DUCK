using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> getProgress = new();

    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (!getProgress.ContainsKey(questSO))
        {
            getProgress[questSO] = new Dictionary<QuestObjective, int>();
        }
        var progressDict = getProgress[questSO];
        int newAmount = 0;
         
        if (objective.targetItem != null)
        {
            //newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
        }
        



        if (!getProgress[questSO].ContainsKey(objective))
        {
            getProgress[questSO][objective] = 0;
        }
        getProgress[questSO][objective] += 1; 
    }
    public string GetProgressText(QuestSO questSO, QuestObjective objective)
    {
        int currentAmount = 0;

        if (currentAmount >= objective.requiredAmount)
            return "Complete ";

        else if (objective.targetItem != null)
            return $"{currentAmount}/{objective.requiredAmount}";
        else
            return "In progress";
    }
}
