using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public GameObject kepregenyObject;
    public GameObject itemsObject;

    void Start()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(MenuController.isDialogueEnabled);

        if (kepregenyObject != null)
            kepregenyObject.SetActive(MenuController.isKepregenyEnabled);

        if (itemsObject != null)
            itemsObject.SetActive(MenuController.isItemsEnabled);
        
        if (MenuController.isKepregenyEnabled)
        {
            FindObjectOfType<VideoUiController>().PlayIntro();
        }
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}