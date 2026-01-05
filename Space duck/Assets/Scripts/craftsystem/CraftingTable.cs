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

    [Header("Recept Könyv UI")]
    public GameObject recipeBookPanel;      // A bal oldali panel (Scroll View-val)
    public Transform recipeContainer;       // A Scroll View 'Content' objektuma
    public GameObject recipeCardPrefab;     // A kártya Prefab (ikon + név + gomb)
    private bool recipeBookOpen = false;

    [Header("Receptek")]
    public List<TableRecipeSO> recipes;     // Ide húzd be az összes ScriptableObject receptet

    void Start()
    {
        // Játékos megkeresése a távolságméréshez
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Receptkönyv kártyáinak legenerálása az indításkor
        GenerateRecipeList();
    }

    void Update()
    {
        // E gomb nyomásra nyitjuk/zárjuk az asztalt, ha elég közel vagyunk
        if (Input.GetKeyDown(openKey))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= interactionRange)
            {
                if (!craftingPanel.activeSelf) OpenTable();
                else CloseTable();
            }
        }
    }

    // --- RECEPT KÖNYV LOGIKA ---

    void GenerateRecipeList()
    {
        // Meglévõ elemek törlése (hogy ne duplázódjanak)
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }

        // Végigmegyünk a listán és létrehozzuk a kártyákat
        foreach (TableRecipeSO recipe in recipes)
        {
            GameObject newCard = Instantiate(recipeCardPrefab, recipeContainer);
            RecipeUIElement uiElem = newCard.GetComponent<RecipeUIElement>();

            // Beállítjuk a kártya adatait és átadjuk neki ezt a scriptet referenciaként
            if (uiElem != null) uiElem.Setup(recipe, this);
        }
    }

    public void ToggleRecipeBook()
    {
        recipeBookOpen = !recipeBookOpen;
        recipeBookPanel.SetActive(recipeBookOpen);
    }

    // Automatikusan feltölti a slotokat az Inventory-ból, ha van elég alapanyag
    public void FillRecipeSlots(TableRecipeSO recipe)
    {
        int hasA = Inventory.Instance.GetItemQuantity(recipe.inputA);
        int hasB = Inventory.Instance.GetItemQuantity(recipe.inputB);

        if (hasA >= recipe.amountA && hasB >= recipe.amountB)
        {
            // Elõször ami bent van, azt visszaadjuk az inventory-ba
            ClearToInventory(slot1);
            ClearToInventory(slot2);

            // Kivesszük az inventory-ból és betesszük a crafting slotba
            RemoveFromInventoryAndSetSlot(recipe.inputA, recipe.amountA, slot1);
            RemoveFromInventoryAndSetSlot(recipe.inputB, recipe.amountB, slot2);

            Debug.Log($"Recept elõkészítve: {recipe.result.itemName}");
        }
        else
        {
            Debug.LogWarning("Nincs elég alapanyagod ehhez a recepthez!");
        }
    }

    private void RemoveFromInventoryAndSetSlot(InventoryItemSO item, int amount, Slot targetSlot)
    {
        int remainingToRemove = amount;

        foreach (Slot s in Inventory.Instance.allSlots)
        {
            // Fontos: ne a crafting slotból vegye el a tárgyat!
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

    private void ClearToInventory(Slot s)
    {
        if (s.HasItem())
        {
            Inventory.Instance.AddItem(s.GetItem(), s.GetAmount());
            s.ClearSlot();
        }
    }

    // --- CRAFTING FOLYAMAT ---

    public void Craft()
    {
        if (!slot1.HasItem() || !slot2.HasItem()) return;

        InventoryItemSO item1 = slot1.GetItem();
        InventoryItemSO item2 = slot2.GetItem();
        int qty1 = slot1.GetAmount();
        int qty2 = slot2.GetAmount();

        // Megkeressük a megfelelõ receptet
        TableRecipeSO foundRecipe = recipes.Find(r => r.Matches(item1, qty1, item2, qty2));

        if (foundRecipe != null)
        {
            // Levonjuk az alapanyagokat a slotokból
            if (item1 == foundRecipe.inputA && qty1 >= foundRecipe.amountA)
            {
                Consume(slot1, foundRecipe.amountA);
                Consume(slot2, foundRecipe.amountB);
            }
            else
            {
                Consume(slot1, foundRecipe.amountB);
                Consume(slot2, foundRecipe.amountA);
            }

            // Hozzáadjuk a kész tárgyat az inventory-hoz
            Inventory.Instance.AddItem(foundRecipe.result, foundRecipe.resultAmount);
            Debug.Log("Sikeres Crafting!");
        }
        else
        {
            Debug.LogWarning("Nincs érvényes recept ehhez a kombinációhoz!");
        }
    }

    private void Consume(Slot s, int amount)
    {
        int newQty = s.GetAmount() - amount;
        if (newQty <= 0) s.ClearSlot();
        else s.SetItem(s.GetItem(), newQty);
    }

    // --- ABLAK KEZELÉS ---

    public void OpenTable()
    {
        craftingPanel.SetActive(true);
        List<Slot> external = new List<Slot> { slot1, slot2 };
        Inventory.Instance.RegisterExternalSlots(external);
        Inventory.Instance.ToggleInventoryUI(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTable()
    {
        // Záráskor visszaadjuk a slotokban maradt tárgyakat az inventory-ba
        ClearToInventory(slot1);
        ClearToInventory(slot2);

        craftingPanel.SetActive(false);
        recipeBookPanel.SetActive(false);
        recipeBookOpen = false;
        Inventory.Instance.ToggleInventoryUI(false);

        // Ha van ilyen metódusod, regisztráljuk le a slotokat
        List<Slot> external = new List<Slot> { slot1, slot2 };
        Inventory.Instance.UnregisterExternalSlots(external);
    }
}