using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string messageToDisplay; 
    public float duration = 5f;     
    public bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Dialogue.Instance != null)
            {
                Dialogue.Instance.ShowExternalMessage(messageToDisplay, duration);
            }

            if (destroyAfterUse)
            {
                Destroy(gameObject);
            }
        }
    }
}