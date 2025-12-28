using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSlotLog : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;
    
    public QuestSO currentQuest;

    public QuestLogUI questLogUI;

    private void OnValidate()
    {
        if (currentQuest != null)
            SetQuest(currentQuest);
        else gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        QuestManager.OnQuestCompleted += HandleQuestCompleted;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestCompleted -= HandleQuestCompleted;
    }

    private void HandleQuestCompleted(QuestSO completedQuest)
    {
        Debug.Log($"Esemény érkezett a slothoz. Quest: {completedQuest.questName} | Slot questje: {(currentQuest != null ? currentQuest.questName : "NULL")}");

        if (completedQuest == currentQuest)
        {
            Debug.Log("<color=green>EGYEZÉS! Slot törlése.</color>");
            Destroy(this.gameObject);
        }
    }
    public void SetQuest(QuestSO questSO)
    {
        currentQuest = questSO;
        questNameText.text = questSO.questName;

        gameObject.SetActive(true);
    }

    public void OnQuestSlotClicked()
    {
        if (currentQuest != null && questLogUI != null)
        {
            questLogUI.HandleQuestClicked(currentQuest);
        }
    } 
}
