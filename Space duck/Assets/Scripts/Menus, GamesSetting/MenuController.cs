using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI Panelek")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Toggle Referenciák")]
    public Toggle dialogueToggle;
    public Toggle kepregenyToggle;
    public Toggle itemsToggle;

    [Header("Fejlesztõi beállítások")]
    public MonoBehaviour playerMovementScript; 
    private bool isMenuOpen = true;

    public static bool isDialogueEnabled = false;
    public static bool isKepregenyEnabled = false;
    public static bool isItemsEnabled = true;

    public void SetDialogue(bool value) => isDialogueEnabled = value;
    public void SetKepregeny(bool value) => isKepregenyEnabled = value;
    public void SetItems(bool value) => isItemsEnabled = value;
    void Awake()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void Start()
    {
        #if UNITY_EDITOR
                QuickStart();
                return;
        #endif
        if (dialogueToggle != null) dialogueToggle.isOn = isDialogueEnabled;
        if (kepregenyToggle != null) kepregenyToggle.isOn = isKepregenyEnabled;
        if (itemsToggle != null) itemsToggle.isOn = isItemsEnabled;

        Time.timeScale = 0f;
        ShowCursor();
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuickStart()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1); 
            return; 
        }

        Time.timeScale = 1f;
        isMenuOpen = false;

        if (playerMovementScript == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovementScript = player.GetComponent<MonoBehaviour>();
            }
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}