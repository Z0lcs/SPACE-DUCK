using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Create SO/New item")]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;
    public int maxStackSize=1;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
}
 