using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("UI Panelek")]
    public GameObject mainMenuPanel;
    public GameObject dialogueCanvas;
    public GameObject kepregenyObject;
    public GameObject itemsObject;

    [Header("Referenciák")]
    public VideoUiController videoUiController; 

    [Header("Játékos Irányítás")]
    public MonoBehaviour playerMovementScript;

    private bool isDialogueEnabled = false;
    private bool isKepregenyEnabled = false;
    private bool isItemsEnabled = true;
    private bool isMenuOpen = true;

    void Awake()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        if (kepregenyObject != null) kepregenyObject.SetActive(false);
        if (itemsObject != null) itemsObject.SetActive(false);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = false;

        isMenuOpen = true;
    }

    void Start()
    {
        Time.timeScale = 0f;
        ShowCursor();
    }

    void Update()
    {
        if (isMenuOpen)
        {
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            {
                ShowCursor();
            }
        }
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetDialogue(bool value) => isDialogueEnabled = value;
    public void SetKepregeny(bool value) => isKepregenyEnabled = value;
    public void SetItems(bool value) => isItemsEnabled = value;

    public void PlayGame()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(isDialogueEnabled);
        if (kepregenyObject != null) kepregenyObject.SetActive(isKepregenyEnabled);
        if (itemsObject != null) itemsObject.SetActive(isItemsEnabled);

        if (isKepregenyEnabled && videoUiController != null)
        {
            videoUiController.PlayIntro();
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        isMenuOpen = false;

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}