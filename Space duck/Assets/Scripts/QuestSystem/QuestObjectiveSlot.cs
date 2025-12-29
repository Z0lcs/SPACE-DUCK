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
        
        if (isComplete)
        {
            objectiveText.color = Color.green;
            trackingText.color = Color.green;
        }
        else
        {
            objectiveText.color = Color.black;
        }
    }
}
