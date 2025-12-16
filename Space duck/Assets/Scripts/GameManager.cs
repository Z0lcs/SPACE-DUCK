using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public LocationHistoryTrack LocationTrack;

    [HideInInspector] public DialogueHistoryTracker DialogueHistoryTracker;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        LocationTrack = GetComponent<LocationHistoryTrack>();
        if (LocationTrack == null)
        {
            Debug.LogError("HIBA: A GameManager-nek szüksége van egy LocationTracker komponensre!");
        }
        DialogueHistoryTracker = GetComponent<DialogueHistoryTracker>();
        if (DialogueHistoryTracker == null)
        {
            Debug.LogError("HIBA: A GameManager-nek szüksége van egy DialogueHistoryTracker komponensre!");
        }
    }

}