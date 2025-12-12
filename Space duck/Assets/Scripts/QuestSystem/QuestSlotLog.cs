using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSlotLog : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questLevelText;
    
    public QuestSO currentQuest;

    public QuestLogUI questLogUI;

    private void OnValidate()
    {
        if (currentQuest != null)
            SetQuest(currentQuest);
        else gameObject.SetActive(false);
    }

    public void SetQuest(QuestSO questSO)
    {
        currentQuest = questSO;
        questNameText.text = questSO.questName;
        questLevelText.text = "Lv. " + questSO.questLevel.ToString();

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
