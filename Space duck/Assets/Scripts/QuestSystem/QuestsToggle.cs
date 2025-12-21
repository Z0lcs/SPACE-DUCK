using UnityEngine;

public class QuestsToggle : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.K;

    [SerializeField] private MonoBehaviour cameraMovementScript;
    [SerializeField] private MonoBehaviour playerMovementScript;
    public GameObject uiElementToToggle;
    public GameObject uiElementNotToToggle;

    private bool isCursorVisible;

    void Start()
    {
        uiElementToToggle.SetActive(false);
        isCursorVisible = false;
        SetCursorState(isCursorVisible);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isCursorVisible = !isCursorVisible;
            SetCursorState(isCursorVisible);
        }
    }

    void SetCursorState(bool visible)
    {
        if (visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;

            if (cameraMovementScript != null) cameraMovementScript.enabled = false;
            if (playerMovementScript != null) playerMovementScript.enabled = false;
            uiElementToToggle.SetActive(true);
            uiElementNotToToggle.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;

            if (cameraMovementScript != null) cameraMovementScript.enabled = true;
            if (playerMovementScript != null) playerMovementScript.enabled = true;
            uiElementToToggle.SetActive(false);
            uiElementNotToToggle.SetActive(true);
        }
    }
}