//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class RecipeUIElement : MonoBehaviour
//{
//    public Image resultIcon;
//    public TextMeshProUGUI recipeName;
//    private TableRecipeSO currentRecipe;
//    private CraftingTable table;

//    public void Setup(TableRecipeSO recipe, CraftingTable owner)
//    {
//        currentRecipe = recipe;
//        table = owner;

//        if (recipe.result != null)
//        {
//            // IDE ÍRD AZT A NEVET, AMIT AZ InventoryItemSO-BAN HASZNÁLSZ (pl. icon vagy sprite)
//            resultIcon.sprite = recipe.result.icon;

//            // IDE PEDIG A NÉV VÁLTOZÓJÁT (pl. itemName vagy Name)
//            recipeName.text = recipe.result.itemName;
//        }
//    }

//    public void OnClickRecipe()
//    {
//        table.FillRecipeSlots(currentRecipe);
//    }
//}