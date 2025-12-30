using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class TableRecipeSO : ScriptableObject
{
    public InventoryItemSO inputA;
    public int amountA;
    public InventoryItemSO inputB;
    public int amountB;

    public InventoryItemSO result;
    public int resultAmount;

    public bool Matches(InventoryItemSO item1, int qty1, InventoryItemSO item2, int qty2)
    {
        if (item1 == null || item2 == null) return false;

        bool variantA = (item1 == inputA && qty1 >= amountA) && (item2 == inputB && qty2 >= amountB);
        bool variantB = (item1 == inputB && qty1 >= amountB) && (item2 == inputA && qty2 >= amountA);

        return variantA || variantB;
    }
}