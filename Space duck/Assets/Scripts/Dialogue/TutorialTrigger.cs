using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string messageToDisplay; // Itt írhatod be a szöveget az Inspectorban
    public float duration = 5f;     // Meddig legyen kint
    public bool destroyAfterUse = true; // Csak egyszer jelenjen meg?

    private void OnTriggerEnter(Collider other)
    {
        // Ellenõrizzük, hogy a játékos lépett-e be
        if (other.CompareTag("Player"))
        {
            TutorialManager manager = FindObjectOfType<TutorialManager>();
            if (manager != null)
            {
                manager.ShowExternalMessage(messageToDisplay, duration);
            }

            if (destroyAfterUse)
            {
                Destroy(gameObject); // Törli a triggert, hogy ne aktiválódjon többször
            }
        }
    }
}