using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Ingredient
{
    public InventoryItemSO item;
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class TableRecipeSO : ScriptableObject
{
    public string itemName;
    public List<Ingredient> ingredients; // Itt bármennyi alapanyagot megadhatsz
    public InventoryItemSO result;
    public int resultAmount = 1;

    // Ellenõrzi, hogy a megadott slot-listában van-e elég mindenbõl
    public bool CanCraft(List<Slot> availableSlots)
    {
        foreach (var ingredient in ingredients)
        {
            int foundAmount = 0;
            foreach (var slot in availableSlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                    foundAmount += slot.GetAmount();
            }
            if (foundAmount < ingredient.amount) return false;
        }
        return true;
    }
}