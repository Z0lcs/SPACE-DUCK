using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 4f;
    public LayerMask interactableLayer;
    public GameObject interactUI;
    public GameObject interactUI2;

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool showUI = false; 
        bool showUI2 = false; 

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            VideoUiController vController = Object.FindAnyObjectByType<VideoUiController>();

            if (hit.collider.CompareTag("Calendar"))
            {
                showUI = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SimpleCalendarFlip calendar = hit.collider.GetComponentInChildren<SimpleCalendarFlip>();
                    if (calendar != null) calendar.FlipPage();
                }
            }
            else if (hit.collider.CompareTag("VideoPlayer"))
            {
                if (vController != null && vController.isQuestCompleted)
                {
                    showUI2 = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        vController.PlayQuestVideo();
                    }
                }
            }
        }

        if (interactUI != null)
        {
            interactUI.SetActive(showUI);
        }
        if (interactUI2 != null)
        {
            interactUI2.SetActive(showUI2);
        }
    }
}