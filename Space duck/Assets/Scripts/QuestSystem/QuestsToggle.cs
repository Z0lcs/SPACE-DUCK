using UnityEngine;
using System.Collections.Generic;

public class QuestsToggle : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.K;

    [Header("UI Elements")]
    public GameObject questPanel;

    [Tooltip("Minden UI elem, amit el kell rejteni, ha a Quest panel nyitva van")]
    public List<GameObject> hudElementsToHide;

    private bool isQuestPanelOpen;
    private float lastToggleTime; 
    private const float cooldownTime = 0.2f;

    void Start()
    {
        isQuestPanelOpen = false;
        if (questPanel != null) questPanel.SetActive(false);
        SetCursorState(false);
    }

    void Update()
    {
        if (isQuestPanelOpen)
        {
            if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleQuestUI(false);
            }
            return; 
        }

        if (Input.GetKeyDown(toggleKey) && Time.timeScale > 0)
        {
            if (Time.unscaledTime > lastToggleTime + cooldownTime)
            {
                ToggleQuestUI(true);
                lastToggleTime = Time.unscaledTime;
            }
        }
    }

    public void ToggleQuestUI(bool isOpen)
    {
        if (questPanel == null) return;

        isQuestPanelOpen = isOpen; 
        questPanel.SetActive(isOpen);

        SetCursorState(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;

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