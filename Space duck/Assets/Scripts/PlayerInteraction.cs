using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 4f;
    public LayerMask interactableLayer;
    public GameObject interactUI;

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            if (interactUI != null) interactUI.SetActive(true);

            if (hit.collider.CompareTag("Calendar") && Input.GetKeyDown(KeyCode.E))
            {
                SimpleCalendarFlip calendar = hit.collider.GetComponentInChildren<SimpleCalendarFlip>();
                if (calendar != null)
                {
                    calendar.FlipPage();
                }
            }
        }
        else
        {
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}