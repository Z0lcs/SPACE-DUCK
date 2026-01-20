using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [Header("Interakció Beállítások")]
    public float interactionRange = 3f;
    public KeyCode openKey = KeyCode.E;
    private Transform player;

    [Header("UI & Slots")]
    public GameObject craftingPanel;
    public List<Slot> containerSlots; // A gép belsõ slotjai (amiket a jobb oldalra tettél)
    public List<TableRecipeSO> recipes; // A receptek listája (SO assetek)

    [Header("UI Automatikus Generálás")]
    public GameObject recipeButtonPrefab; // Az "Engram" gomb prefabja
    public Transform recipeContainer;     // A Scroll View 'Content' objektuma

    void Start()
    {
        // Játékos keresése a távolság ellenõrzéséhez
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Receptek legenerálása a UI-ra az indításkor
        GenerateRecipes();
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

    // --- CRAFTING LOGIKA ---

    public void Craft(TableRecipeSO recipe)
    {
        if (recipe == null) return;

        // Ellenõrizzük, hogy a gép belsõ slotjaiban (containerSlots) van-e elég anyag
        if (recipe.CanCraft(containerSlots))
        {
            // 1. Alapanyagok levonása
            foreach (var ingredient in recipe.ingredients)
            {
                ConsumeItems(ingredient.item, ingredient.amount);
            }

            // 2. Eredmény hozzáadása a gép inventory-jába
            AddItemToContainer(recipe.result, recipe.resultAmount);

            Debug.Log($"{recipe.itemName} sikeresen legyártva!");
        }
        else
        {
            Debug.Log("Nincs elég alapanyag a gépben!");
        }
    }

    private void ConsumeItems(InventoryItemSO item, int amount)
    {
        int remainingToConsume = amount;
        foreach (var slot in containerSlots)
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

    private void AddItemToContainer(InventoryItemSO item, int amount)
    {
        // Megpróbáljuk meglévõ stackhez adni a gépen belül
        foreach (var slot in containerSlots)
        {
            if (slot.HasItem() && slot.GetItem() == item)
            {
                slot.SetItem(item, slot.GetAmount() + amount);
                return;
            }
        }

        // Ha nincs stack, keresünk egy üres helyet a gépben
        foreach (var slot in containerSlots)
        {
            if (!slot.HasItem())
            {
                slot.SetItem(item, amount);
                return;
            }
        }
    }

    // --- UI ÉS GENERÁLÁS ---

    public void GenerateRecipes()
    {
        // Töröljük a meglévõ gombokat
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }

        // Új gombok létrehozása minden recepthez
        foreach (TableRecipeSO recipe in recipes)
        {
            GameObject newBtn = Instantiate(recipeButtonPrefab, recipeContainer);
            RecipeButtonUI buttonScript = newBtn.GetComponent<RecipeButtonUI>();

            if (buttonScript != null)
            {
                buttonScript.Setup(recipe, this);
            }
        }
    }

    public void OpenTable()
    {
        craftingPanel.SetActive(true);
        // Regisztráljuk a gép slotjait az inventory rendszerbe
        Inventory.Instance.RegisterExternalSlots(containerSlots);
        Inventory.Instance.ToggleInventoryUI(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTable()
    {
        craftingPanel.SetActive(false);
        // Leiratkozunk a slotokról
        Inventory.Instance.UnregisterExternalSlots(containerSlots);
        Inventory.Instance.ToggleInventoryUI(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}