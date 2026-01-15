using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("UI Panelek")]
    public GameObject mainMenuPanel;
    public GameObject dialogueCanvas;
    public GameObject kepregenyObject;
    public GameObject itemsObject; // ÚJ: Az Items objektum helye

    [Header("Játékos Irányítás")]
    public MonoBehaviour playerMovementScript;

    private bool isDialogueEnabled = true;
    private bool isKepregenyEnabled = true;
    private bool isItemsEnabled = true; // ÚJ állapot
    private bool isMenuOpen = true;

    void Awake()
    {
        // Mindent lekapcsolunk az indítás pillanatában
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
        // Csak akkor kényszerítjük a kurzort, ha nyitva a menü
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

    // Toggle függvények a UI-hoz
    public void SetDialogue(bool value) => isDialogueEnabled = value;
    public void SetKepregeny(bool value) => isKepregenyEnabled = value;
    public void SetItems(bool value) => isItemsEnabled = value; // ÚJ

    public void PlayGame()
    {
        // Beállítások érvényesítése
        if (dialogueCanvas != null) dialogueCanvas.SetActive(isDialogueEnabled);
        if (kepregenyObject != null) kepregenyObject.SetActive(isKepregenyEnabled);
        if (itemsObject != null) itemsObject.SetActive(isItemsEnabled);

        // Menü bezárása
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        isMenuOpen = false;

        // Játék indítása
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Time.timeScale = 1f;

        // Kurzor elrejtése a játékhoz
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}