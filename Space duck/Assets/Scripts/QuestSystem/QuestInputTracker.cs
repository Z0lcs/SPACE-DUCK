using UnityEngine;
using System.Collections.Generic;

public class QuestInputTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestManager questManager;

    public List<QuestSO> activeQuests = new List<QuestSO>();

    void Update()
    {
        foreach (QuestSO quest in activeQuests)
        {
            foreach (QuestObjective objective in quest.questObjectives)
            {
                if (objective.targetKey != null)
                {
                    if (objective.targetKey.IsAnyKeyPressed())
                    {
                        questManager.UpdateObjectiveProgress(quest, objective);
                    }
                }
            }
        }
    }

    public void TrackQuest(QuestSO quest)
    {
        if (!activeQuests.Contains(quest)) activeQuests.Add(quest);
    }

    public void UntrackQuest(QuestSO quest)
    {
        if (activeQuests.Contains(quest)) activeQuests.Remove(quest);
    }
}