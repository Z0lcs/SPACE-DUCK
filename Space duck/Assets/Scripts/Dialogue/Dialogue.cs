using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Referenciák")]
    public TextMeshProUGUI tutorialText;
    public Image characterPortrait;
    public GameObject dialoguePanel;

    private int step = 0;
    private bool tutorialActive = true;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartCoroutine(TypeText("Üdv a világban, vándor! Használd a WASD billentyûket a mozgáshoz!"));
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (step == 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            NextStep("Nagyszerû! Most nyomd meg a SPACE-t az ugráshoz!");
        }
        else if (step == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            NextStep("Sikerült! Készen állsz a kalandra. Sok szerencsét!");
            tutorialActive = false;
            Invoke("HidePanel", 4f);
        }
    }

    // Ezt hívja meg a Trigger és a Quest rendszer is
    public void ShowExternalMessage(string message, float displayTime = 4f)
    {
        StopAllCoroutines();
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartCoroutine(TypeText(message));

        CancelInvoke("HidePanel");
        Invoke("HidePanel", displayTime);
    }

    void NextStep(string message)
    {
        step++;
        StopAllCoroutines();
        StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }
    }

    void HidePanel()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}