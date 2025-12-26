using UnityEngine;
using System.Collections.Generic;

public class QuestsToggle : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.K;

    [Header("Scripts to Disable")]
    [SerializeField] private MonoBehaviour cameraMovementScript;
    [SerializeField] private MonoBehaviour playerMovementScript;

    [Header("UI Elements")]
    public GameObject questPanel; 

    [Tooltip("Minden UI elem, amit el kell rejteni, ha a Quest panel nyitva van")]
    public List<GameObject> hudElementsToHide; 

    private bool isQuestPanelOpen;

    void Start()
    {
        isQuestPanelOpen = false;
        questPanel.SetActive(false);
        SetCursorState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isQuestPanelOpen = !isQuestPanelOpen;
            ToggleQuestUI(isQuestPanelOpen);
        }
    }

    void ToggleQuestUI(bool isOpen)
    {
        questPanel.SetActive(isOpen);

        SetCursorState(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;

        if (cameraMovementScript != null) cameraMovementScript.enabled = !isOpen;
        if (playerMovementScript != null) playerMovementScript.enabled = !isOpen;

        foreach (GameObject hudElement in hudElementsToHide)
        {
            if (hudElement != null)
            {
                hudElement.SetActive(!isOpen);
            }
        }
    }

    void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}