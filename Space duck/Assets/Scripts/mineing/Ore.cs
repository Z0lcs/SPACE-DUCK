using UnityEngine;

public class Ore : MonoBehaviour
{
    public OreData data;
    public GameObject hitMarkerPrefab;

    private float currentHealth;
    private GameObject currentMarker;

    void Start()
    {
        currentHealth = data.maxHealth;
    }

    public void GetHit(float powerMultiplier, bool hitMarker)
    {
        // Sebzés számítás: Alap + (Bónusz ha eltalálta a pontot) * Csúszka ereje
        float baseDamage = hitMarker ? 40f : 15f;
        float totalDamage = baseDamage * (1f + powerMultiplier);

        currentHealth -= totalDamage;
        Debug.Log($"{data.oreName} élete: {currentHealth}");

        if (currentMarker != null) Destroy(currentMarker);

        if (currentHealth <= 0)
        {
            Debug.Log($"{data.oreName} kibányászva! Kapott mennyiség: {data.yieldAmount}");
            Destroy(gameObject);
            return;
        }

        SpawnMarker();
    }

    void SpawnMarker()
    {
        // Egy véletlenszerû pont az érc felületén
        Vector3 randomOffset = Random.onUnitSphere * 0.5f;
        currentMarker = Instantiate(hitMarkerPrefab, transform.position + randomOffset, Quaternion.identity);
        currentMarker.transform.parent = transform;

        // Szín beállítása az adatlapról
        currentMarker.GetComponent<Renderer>().material.color = data.markerColor;
    }
}