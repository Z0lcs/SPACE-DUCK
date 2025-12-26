using UnityEngine;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject detailsPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;

    [Header("Objectives")]
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;

    private QuestSO questSO;

    void Start()
    {
        if (detailsPanel != null) detailsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        DisplayObjectives();
    }

    public void HandleQuestClicked(QuestSO clickedQuest)
    {
        this.questSO = clickedQuest;

        if (detailsPanel != null) detailsPanel.SetActive(true);

        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;

        DisplayObjectives();
    }

    private void DisplayObjectives()
    {
        if (questSO == null || QuestManager.Instance == null) return;

        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSO.questObjectives.Count)
            {
                var objective = questSO.questObjectives[i];

                int currentAmount = QuestManager.Instance.GetCurrentAmount(questSO, objective);
                string progressText = QuestManager.Instance.GetProgressText(questSO, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;

                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjectives(objective.description, progressText, isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResetSelection()
    {
        questSO = null;
        if (detailsPanel != null) detailsPanel.SetActive(false);
    }
}