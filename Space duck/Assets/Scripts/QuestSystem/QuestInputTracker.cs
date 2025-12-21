using UnityEngine;
using System.Collections.Generic;

public class QuestInputTracker : MonoBehaviour
{
    public static QuestInputTracker Instance { get; private set; } 
    public List<QuestSO> activeQuests = new List<QuestSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this); 
        }
    }

    void Update()
    {
        if (QuestManager.Instance == null) return;

        foreach (QuestSO quest in activeQuests)
        {
            foreach (QuestObjective objective in quest.questObjectives)
            {
                if (objective.targetKey != null)
                {
                    QuestManager.Instance.UpdateObjectiveProgress(quest, objective);
                }
            }
        }
    }

    public void TrackQuest(QuestSO quest) => activeQuests.Add(quest);
}