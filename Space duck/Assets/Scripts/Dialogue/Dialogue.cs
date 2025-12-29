using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("UI Referenciák")]
    public TextMeshProUGUI tutorialText;
    public GameObject dialoguePanel;
    public GameObject inventoryPanel;
    public GameObject questPanel;

    private int step = 0;
    private bool tutorialActive = true;
    private float stepTimer = 0f;

    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartTutorialStep("Üdv a világban! Használd a W, A, S, D billentyûket a mozgáshoz.");
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (stepTimer > 0) stepTimer -= Time.unscaledDeltaTime;

        bool isQuestOpen = questPanel != null && questPanel.activeInHierarchy;

        switch (step)
        {
            case 0 when (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0):
                NextStep("Nagyon jó! Most próbálj meg ugrani egyet a SPACE billentyûvel.");
                break;

            case 1 when Input.GetKeyDown(KeyCode.Space):
                NextStep("Szuper! Most már tudsz mozogni.");
                stepTimer = 1.5f;
                break;

            case 2 when stepTimer <= 0:
                NextStep("Az inventoryt a TAB billentyûvel tudod megnyitni.");
                stepTimer = 0.5f;
                break;

            case 3: 
                if (Input.GetKeyDown(KeyCode.Tab) && stepTimer <= 0)
                {
                    NextStep("Ez az inventory! Zárd be a TAB-al a folytatáshoz.");
                    stepTimer = 0.5f;
                }
                break;

            case 4: 
                if (Input.GetKeyDown(KeyCode.Tab) && stepTimer <= 0)
                {
                    NextStep("A küldetéseket a K billentyûvel érheted el.");
                    stepTimer = 0.5f;
                }
                break;

            case 5 when Input.GetKeyDown(KeyCode.K) && stepTimer <= 0:
                NextStep("Itt találod a feladatokat. Zárd be a K-val a folytatáshoz.");
                stepTimer = 0.5f;
                break;

            case 6 when !isQuestOpen && stepTimer <= 0:
                NextStep("Az eszközök felvételéhez használd az E billentyût.");
                stepTimer = 1.0f;
                break;

            case 7 when Input.GetKeyDown(KeyCode.E) && stepTimer <= 0:
                NextStep("A dobozok kinyitásához használd a bal egérgombot.");
                stepTimer = 1.0f;
                break;

            case 8 when Input.GetMouseButtonDown(0) && stepTimer <= 0:
                NextStep("Az étel elfogyasztásához használd a jobb egérgombot.");
                stepTimer = 1.0f;
                break;

            case 9 when Input.GetMouseButtonDown(1) && stepTimer <= 0:
                NextStep("Most már szinte mindent tudsz! Jó játékot!");
                Invoke("FinishTutorial", 3f);
                break;
        }
    }


    void NextStep(string message)
    {
        step++;
        StartTutorialStep(message);
    }

    void StartTutorialStep(string message)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }

    public void ShowExternalMessage(string message, float displayTime = 4f)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartTutorialStep(message);
        if (!tutorialActive) Invoke("HidePanel", displayTime);
    }

    void FinishTutorial()
    {
        tutorialActive = false;
        HidePanel();
    }

    void HidePanel() { if (dialoguePanel != null) dialoguePanel.SetActive(false); }
}