using UnityEngine;

public class LocationZone : MonoBehaviour
{
    [Header("Melyik helyszín ez?")]
    [SerializeField] private LocationSO locationData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (locationData != null)
            {
                GameManager.Instance.LocationTrack.RecordLocation(locationData);
                gameObject.SetActive(false);
            }
        }
    }
}