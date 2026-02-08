using UnityEngine;

public class Ore : MonoBehaviour
{
    [Header("Érc Beállítások")]
    public InventoryItemSO oreItemSO; // Húzd be ide az érc InventoryItemSO-ját!
    public OreData oreData;          // Az OreData SO az életerõhöz és mennyiséghez

    private float currentHealth;

    [Header("HitMarker")]
    public GameObject hitMarkerPrefab;
    private GameObject currentMarker;

    void Start()
    {
        if (oreData != null)
        {
            currentHealth = oreData.maxHealth; // OreData-ból vesszük az életerõt
        }
        SpawnMarker();
    }

    public void GetHit(float damageMultiplier, bool hitMarker)
    {
        float baseDamage = 20f;
        float sliderDamage = baseDamage * damageMultiplier;
        float finalDamage = hitMarker ? (sliderDamage * 2f) : sliderDamage;

        currentHealth -= finalDamage;

        if (hitMarker) Debug.Log("<color=cyan>[HITMARKER TALÁLAT!]</color>");

        if (currentHealth <= 0)
        {
            GiveReward(); // Érc hozzáadása az inventoryhoz
            return;
        }

        if (currentMarker != null) Destroy(currentMarker);
        SpawnMarker();
    }

    private void GiveReward()
    {
        if (Inventory.Instance != null && oreItemSO != null && oreData != null)
        {
            // Meghívjuk az Inventory.Logic.cs-ben lévõ AddItem-et
            Inventory.Instance.AddItem(oreItemSO, oreData.yieldAmount);
            Debug.Log($"<color=green>{oreData.oreName} hozzáadva!</color>");
        }

        Destroy(gameObject);
    }

    private void SpawnMarker()
    {
        // Marker spawnolási logika...
    }
}