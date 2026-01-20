using UnityEngine;
using UnityEngine.UI;

public class RecipeButtonUI : MonoBehaviour
{
    public Image iconImage;      // Ebbe húzd be a gombodon lévõ ikon képet
    private TableRecipeSO recipe; // Ezt a receptet tárolja el a gomb
    private CraftingTable table;  // Referencia a géphez

    // Ezt hívja meg a CraftingTable, amikor létrehozza a gombot
    public void Setup(TableRecipeSO newRecipe, CraftingTable newTable)
    {
        recipe = newRecipe;
        table = newTable;

        if (recipe.result != null)
        {
            iconImage.sprite = recipe.result.icon; // Az ikon beállítása a recept eredményére
        }
    }

    // Ezt a függvényt kösd be a Button OnClick() eseményéhez!
    public void OnClick()
    {
        if (table != null && recipe != null)
        {
            table.Craft(recipe); // Szólunk a gépnek, hogy gyártsa le ezt
        }
    }
}