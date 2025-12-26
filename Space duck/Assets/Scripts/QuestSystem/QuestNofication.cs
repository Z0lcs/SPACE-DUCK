using UnityEngine;
using TMPro; // Kell a szöveghez

public class QuestNofication : MonoBehaviour
{
    public GameObject notificationPanel; 
    public TextMeshProUGUI questNameText;
    public AudioSource audioSource; 
    public AudioClip completeSound;

    void Start() => notificationPanel.SetActive(false);

    void OnEnable() => QuestManager.OnQuestCompleted += ShowNotification;
    void OnDisable() => QuestManager.OnQuestCompleted -= ShowNotification;

    void ShowNotification(QuestSO questSO)
    {
        if (audioSource != null && completeSound != null)
        {
            audioSource.PlayOneShot(completeSound);
        }
        notificationPanel.SetActive(true);
        questNameText.text = questSO.questName;

        Invoke("HideNotification", 3f);
    }

    void HideNotification() => notificationPanel.SetActive(false);
}