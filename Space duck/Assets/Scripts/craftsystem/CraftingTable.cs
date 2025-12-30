using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [Header("Beállítások")]
    public float interactionRange = 3f;
    public KeyCode openKey = KeyCode.E;
    private Transform player;

    [Header("UI Referenciák")]
    public GameObject craftingPanel;
    public Slot slot1;
    public Slot slot2;

    [Header("Receptek")]
    public List<TableRecipeSO> recipes;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
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
            // Meghatározzuk a helyes levonást a típusok alapján
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

            Inventory.Instance.AddItem(foundRecipe.result, foundRecipe.resultAmount);
            Debug.Log("Sikeres Crafting!");
        }
        else
        {
            Debug.LogWarning("Nincs érvényes recept vagy kevés az alapanyag!");
        }
    }

    // CSAK EGYSZER SZEREPELHET A FÁJLBAN!
    private void Consume(Slot s, int amount)
    {
        int newQty = s.GetAmount() - amount;
        if (newQty <= 0)
            s.ClearSlot();
        else
            s.SetItem(s.GetItem(), newQty);
    }

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
        craftingPanel.SetActive(false);
        Inventory.Instance.ToggleInventoryUI(false);
    }
}