using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    public void HandleQuestClicked(QuestSO questSO)
    {
        Debug.Log($"===Quest clicked: {questSO.questName}===");

        foreach (var objective in questSO.questObjectives)
        {
            questManager.UpdateObjectiveProgress(questSO, objective);
            Debug.Log($"Objective: {objective.description} => {questManager.GetProgressText(questSO, objective)}");
        } 
    }
}
