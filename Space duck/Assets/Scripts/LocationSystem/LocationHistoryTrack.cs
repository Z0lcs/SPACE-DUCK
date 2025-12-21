using UnityEngine;
using System.Collections.Generic;

public class LocationHistoryTrack : MonoBehaviour
{
    private readonly HashSet<LocationSO> locationsVisited = new HashSet<LocationSO>();

    public void RecordLocation(LocationSO locationSO)
    {
        if (locationsVisited.Add(locationSO))
        {
            Debug.Log("Just visited: " + locationSO.displayName);
        }
    }

    public bool HasVisited(LocationSO locationSO)
    {
        return locationsVisited.Contains(locationSO);
    }
}
