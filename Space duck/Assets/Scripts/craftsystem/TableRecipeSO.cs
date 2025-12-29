using UnityEngine;

[CreateAssetMenu(fileName = "New Table Recipe", menuName = "Inventory/Table Recipe")]
public class TableRecipeSO : ScriptableObject
{
    public InventoryItemSO inputA;
    public int amountA = 1;
    public InventoryItemSO inputB;
    public int amountB = 1;

    public InventoryItemSO result;
    public int resultAmount = 1;

   
    public bool Matches(InventoryItemSO item1, int qty1, InventoryItemSO item2, int qty2)
    {
        return (item1 == inputA && qty1 >= amountA && item2 == inputB && qty2 >= amountB) ||
               (item1 == inputB && qty1 >= amountB && item2 == inputA && qty2 >= amountA);
    }
}