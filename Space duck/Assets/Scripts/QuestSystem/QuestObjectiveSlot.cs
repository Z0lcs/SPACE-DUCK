using UnityEngine;
using TMPro;

public class QuestObjectiveSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text trackingText;

    public void RefreshObjectives(string description, string progressText, bool isComplete)
    {
        objectiveText.text = description;
        trackingText.text = progressText;
    }
}
