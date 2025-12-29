using UnityEngine;

public class LocationZone : MonoBehaviour
{
    [Header("Melyik helyszín ez?")]
    [SerializeField] private LocationSO locationData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (locationData != null)
            {
                GameManager.Instance.LocationTrack.RecordLocation(locationData);

                if (QuestManager.Instance != null && QuestInputTracker.Instance != null)
                {
                    foreach (QuestSO quest in QuestInputTracker.Instance.activeQuests)
                    {
                        foreach (QuestObjective obj in quest.questObjectives)
                        {
                            if (obj.targetLocation == locationData)
                                QuestManager.Instance.UpdateObjectiveProgress(quest, obj);
                        }
                    }
                }
            }
        }
    }
}