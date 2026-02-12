using UnityEngine;

public enum ItemType { Resource, Consumable }

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Create SO/New item")]
public class InventoryItemSO : ScriptableObject
{
    [Header("Smelting Settings")]
    public bool isSmeltable;
    public InventoryItemSO smeltedResult;
    public float smeltingTime = 10f;
    public float fuelValue = 25f;

    [Header("General Settings")]
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;
    public int maxStackSize = 1;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;

    [Header("Usage Settings")]
    public ItemType itemType;
    [Tooltip("Ennyi egység kaját tölt vissza evéskor")]
    public int hungerRestoreValue = 1; // Új változó a visszatöltés mértékéhez

    public virtual bool Use()
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                // Megkeressük a HungerManagert a Singletonon keresztül
                if (HungerManager.Instance != null)
                {
                    // Itt hívjuk meg a korábban megírt IncreaseHunger függvényt
                    HungerManager.Instance.IncreaseHunger(hungerRestoreValue);

                    Debug.Log(itemName + " elhasználva. Visszatöltve: " + hungerRestoreValue);
                    return true; // Jelezzük az Inventory-nak, hogy a tárgy sikeresen el lett használva
                }
                else
                {
                    Debug.LogError("Hiba: Nincs HungerManager a jelenetben (Scene)!");
                    return false;
                }

            case ItemType.Resource:
                Debug.Log(itemName + " egy nyersanyag, nem ehetõ.");
                return false;

            default:
                return false;
        }
    }
}