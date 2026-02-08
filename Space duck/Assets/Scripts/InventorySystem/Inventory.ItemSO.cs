using UnityEngine;

public enum ItemType { Resource, Consumable }

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Create SO/New item")]
public class InventoryItemSO : ScriptableObject
{
    [Header("Smelting Settings")]
    public bool isSmeltable;
    public InventoryItemSO smeltedResult;
    public float smeltingTime = 10f; // Ezt hiányolta a fordító
    public float fuelValue = 25f;    // Ennyi hõt ad, ha üzemanyagként használod

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

    public virtual bool Use()
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                // Itt hivatkozhatsz a HungerManager-re, ha létezik
                return true;
            default:
                return false;
        }
    }
}