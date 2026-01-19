using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [Header("Interakció Beállítások")]
    public float interactionRange = 3f;
    public KeyCode openKey = KeyCode.E;
    private Transform player;

    [Header("UI Referenciák")]
    public GameObject craftingPanel;
    public Slot slot1;
    public Slot slot2;

    //[Header("Recept Könyv UI")]
    //public GameObject recipeBookPanel;
    //public Transform recipeContainer;
    //public GameObject recipeCardPrefab;
    //private bool recipeBookOpen = false;

    [Header("Receptek")]
    public List<TableRecipeSO> recipes;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        //GenerateRecipeList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseTable();
        if (player == null || !Input.GetKeyDown(openKey)) return;
        
        

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= interactionRange)
        {
            if (!craftingPanel.activeSelf) OpenTable();
            else CloseTable();
        }
    }

    // --- JAVÍTOTT CRAFTING LOGIKA ---

    public void Craft()
    {
        if (!slot1.HasItem() || !slot2.HasItem()) return;

        InventoryItemSO item1 = slot1.GetItem();
        InventoryItemSO item2 = slot2.GetItem();
        int qty1 = slot1.GetAmount();
        int qty2 = slot2.GetAmount();

        TableRecipeSO foundRecipe = recipes.Find(r => r.Matches(item1, qty1, item2, qty2));

        if (foundRecipe != null)
        {
            // Kiszámoljuk mennyit kell levonni
            int take1 = (item1 == foundRecipe.inputA) ? foundRecipe.amountA : foundRecipe.amountB;
            int take2 = (item2 == foundRecipe.inputB) ? foundRecipe.amountB : foundRecipe.amountA;

            // ELLENÕRZÉS: Írassuk ki a konzolra!
            Debug.Log($"LEVONÁS INDUL: Slot1-bõl {take1}, Slot2-bõl {take2}");

            // Levonjuk slot 1-bõl
            int newQty1 = qty1 - take1;
            if (newQty1 <= 0) slot1.ClearSlot();
            else slot1.SetItem(item1, newQty1);

            // Levonjuk slot 2-bõl
            int newQty2 = qty2 - take2;
            if (newQty2 <= 0) slot2.ClearSlot();
            else slot2.SetItem(item2, newQty2);

            // Eredmény hozzáadása
            Inventory.Instance.AddItem(foundRecipe.result, foundRecipe.resultAmount);
            Debug.Log("Sikeres Crafting!");
        }
    }

    private void DoConsume(Slot s, int amount)
    {
        int startAmount = s.GetAmount();
        int remaining = startAmount - amount;

        if (remaining <= 0)
        {
            s.ClearSlot();
        }
        else
        {
            // Itt küldjük el az új adatokat a slotnak
            s.SetItem(s.GetItem(), remaining);
        }
    }

    // --- RECEPT KÖNYV ÉS FILL LOGIKA ---

    public void FillRecipeSlots(TableRecipeSO recipe)
    {
        int hasA = Inventory.Instance.GetItemQuantity(recipe.inputA);
        int hasB = Inventory.Instance.GetItemQuantity(recipe.inputB);

        if (hasA >= recipe.amountA && hasB >= recipe.amountB)
        {
            ClearToInventory(slot1);
            ClearToInventory(slot2);

            RemoveFromInventoryAndSetSlot(recipe.inputA, recipe.amountA, slot1);
            RemoveFromInventoryAndSetSlot(recipe.inputB, recipe.amountB, slot2);
        }
    }

    private void RemoveFromInventoryAndSetSlot(InventoryItemSO item, int amount, Slot targetSlot)
    {
        int remainingToRemove = amount;
        foreach (Slot s in Inventory.Instance.allSlots)
        {
            if (s == slot1 || s == slot2) continue;
            if (s.HasItem() && s.GetItem() == item)
            {
                int take = Mathf.Min(s.GetAmount(), remainingToRemove);
                int newAmount = s.GetAmount() - take;
                if (newAmount <= 0) s.ClearSlot();
                else s.SetItem(item, newAmount);
                remainingToRemove -= take;
                if (remainingToRemove <= 0) break;
            }
        }
        targetSlot.SetItem(item, amount);
    }

    // --- SEGÉDFÜGGVÉNYEK ---

    private void ClearToInventory(Slot s)
    {
        if (s.HasItem())
        {
            Inventory.Instance.AddItem(s.GetItem(), s.GetAmount());
            s.ClearSlot();
        }
    }

    //void GenerateRecipeList()
    //{
    //    foreach (Transform child in recipeContainer) Destroy(child.gameObject);
    //    foreach (TableRecipeSO recipe in recipes)
    //    {
    //        GameObject newCard = Instantiate(recipeCardPrefab, recipeContainer);
    //        newCard.GetComponent<RecipeUIElement>()?.Setup(recipe, this);
    //    }
    //}

    //public void ToggleRecipeBook()
    //{
    //    recipeBookOpen = !recipeBookOpen;
    //    recipeBookPanel.SetActive(recipeBookOpen);
    //}

    public void OpenTable()
    {
        craftingPanel.SetActive(true);
        Inventory.Instance.RegisterExternalSlots(new List<Slot> { slot1, slot2 });
        Inventory.Instance.ToggleInventoryUI(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTable()
    {
        ClearToInventory(slot1);
        ClearToInventory(slot2);
        craftingPanel.SetActive(false);
        //recipeBookPanel.SetActive(false);
        //recipeBookOpen = false;
        Inventory.Instance.ToggleInventoryUI(false);
        Inventory.Instance.UnregisterExternalSlots(new List<Slot> { slot1, slot2 });
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}