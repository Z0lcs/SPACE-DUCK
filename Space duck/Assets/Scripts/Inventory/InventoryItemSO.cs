using UnityEngine;

// Típusok meghatározása
public enum ItemType { Resource, Consumable }

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Create SO/New item")]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;
    public int maxStackSize = 1;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;

    [Header("Usage Settings")]
    public ItemType itemType; 

    public virtual bool Use()
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                HungerManager.Instance.IncreaseHunger(1);
                return true;

            default:
                return false;
        }
    }
}