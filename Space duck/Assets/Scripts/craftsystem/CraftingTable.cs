using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [Header("Interakció Beállítások")]
    public float interactionRange = 3f;
    public KeyCode openKey = KeyCode.E;
    private Transform player;

    [Header("UI Beállítások")]
    public GameObject craftingPanel;
    public List<TableRecipeSO> recipes;

    [Header("UI Automatikus Generálás")]
    public GameObject recipeButtonPrefab;
    public Transform recipeContainer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        GenerateRecipes();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseTable();
        if (player == null || !Input.GetKeyDown(openKey)) return;

        if (Vector3.Distance(transform.position, player.position) <= interactionRange)
        {
            if (!craftingPanel.activeSelf) OpenTable();
            else CloseTable();
        }
    }

    public void Craft(TableRecipeSO recipe)
    {
        if (recipe == null) return;

        // Az Inventory allSlots listáját használjuk az ellenõrzéshez
        // Így minden benne van: táska + hotbar
        if (recipe.CanCraft(Inventory.Instance.allSlots))
        {
            // 1. Levonjuk az alapanyagokat
            foreach (var ingredient in recipe.ingredients)
            {
                ConsumeFromInventory(ingredient.item, ingredient.amount);
            }

            // 2. Hozzáadjuk a kész tárgyat a te AddItem metódusoddal
            // Ez automatikusan stackeli az itemet és kezeli a táska telítettségét
            Inventory.Instance.AddItem(recipe.result, recipe.resultAmount);

            // 3. Frissítjük a küldetéseket (mivel változott a készletünk)
            // Az Inventory-dban ez a metódus végzi a Quest frissítést
            Inventory.Instance.NotifyQuestManagerOfInventoryChange();

            Debug.Log($"{recipe.itemName} elkészítve!");
        }
        else
        {
            Debug.Log("Nincs elég alapanyag a táskádban!");
        }
    }

    private void ConsumeFromInventory(InventoryItemSO item, int amount)
    {
        int remainingToConsume = amount;

        // Végigmegyünk az összes sloton (allSlots), amit az Inventory kezel
        foreach (Slot slot in Inventory.Instance.allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == item)
            {
                int available = slot.GetAmount();
                int take = Mathf.Min(available, remainingToConsume);

                int newQty = available - take;
                if (newQty <= 0) slot.ClearSlot();
                else slot.SetItem(item, newQty);

                remainingToConsume -= take;
                if (remainingToConsume <= 0) break;
            }
        }
    }

    // --- UI ÉS NYITÁS ---

    public void OpenTable()
    {
        craftingPanel.SetActive(true);
        // Itt nem regisztrálunk külsõ slotokat, mert nincs saját tárolója a gépnek
        Inventory.Instance.ToggleInventoryUI(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTable()
    {
        craftingPanel.SetActive(false);
        Inventory.Instance.ToggleInventoryUI(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GenerateRecipes()
    {
        foreach (Transform child in recipeContainer) Destroy(child.gameObject);
        foreach (TableRecipeSO recipe in recipes)
        {
            GameObject newBtn = Instantiate(recipeButtonPrefab, recipeContainer);
            RecipeButtonUI buttonScript = newBtn.GetComponent<RecipeButtonUI>();
            if (buttonScript != null) buttonScript.Setup(recipe, this);
        }
    }
}