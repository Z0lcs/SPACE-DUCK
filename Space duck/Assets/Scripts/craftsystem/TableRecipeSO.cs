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
        bool order1 = (item1 == inputA && qty1 >= amountA && item2 == inputB && qty2 >= amountB);
        bool order2 = (item1 == inputB && qty1 >= amountB && item2 == inputA && qty2 >= amountA);
        return order1 || order2;
    }
}