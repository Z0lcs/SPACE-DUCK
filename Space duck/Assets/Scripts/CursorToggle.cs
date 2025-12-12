using UnityEngine;

public class CursorToggle : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.K;

    public MonoBehaviour cameraMovementScript;
    public MonoBehaviour playerMovementScript;

    private bool isCursorVisible;

    void Start()
    {
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

            Debug.Log("Egér kurzor bekapcsolva és a játék szünetel.");
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = 1f;

            if (cameraMovementScript != null) cameraMovementScript.enabled = true;
            if (playerMovementScript != null) playerMovementScript.enabled = true;

            Debug.Log("Egér kurzor kikapcsolva és a játék folytatódik.");
        }
    }
}