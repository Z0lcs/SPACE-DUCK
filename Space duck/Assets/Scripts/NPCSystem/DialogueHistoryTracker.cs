using UnityEngine;
using System.Collections.Generic;

public class DialogueHistoryTracker : MonoBehaviour
{
    private readonly HashSet<ActorSO> spokenNPCs = new HashSet<ActorSO>();

    public void RecordNPC(ActorSO actorSO)
    {
        spokenNPCs.Add(actorSO);
        Debug.Log("Just spoke to: " + actorSO.actorName);
        if (QuestManager.Instance != null && QuestInputTracker.Instance != null)
        {
            foreach (QuestSO quest in QuestInputTracker.Instance.activeQuests)
            {
                foreach (QuestObjective obj in quest.questObjectives)
                {
                    if (obj.targetNPC == actorSO)
                        QuestManager.Instance.UpdateObjectiveProgress(quest, obj);
                }
            }
        }
    }
    public bool HasSpokenWith(ActorSO actorSO)
    {
        return spokenNPCs.Contains(actorSO);
    }
}
