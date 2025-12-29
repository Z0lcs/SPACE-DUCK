using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("Prefabs")]
    public Transform hand;

    [Header("UI Elements")]
    public GameObject hotbarObj;
    public GameObject inventorySlotParent;
    public GameObject container;
    public GameObject hudCanvas;
    public Image dragIcon;

    [Header("Settings")]
    public float pickupRange = 10f;
    public Material highlightMaterial;
    public LayerMask excludeLayers;
    public float equppedOpacity = 0.9f;
    public float normalOpacity = 0.58f;

    private Material originalMaterial;
    private Renderer lookedAtRenderer;
    private Item currentlyLookedAtItem;
    private Chest currentlyLookedAtChest;
    private GameObject currentHandItem;
    private int equippedHotbarIndex = 0;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot dragedSlot = null;
    private bool isDragging = false;
    private Chest currentOpenedChest;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(hotbarSlots);
        allSlots.AddRange(inventorySlots);
    }

    private void Start()
    {
        UpdateHotbarOpacity();
        EquipHandItem();
    }

    void Update()
    {
        // 1. MEGHATÁROZZUK AZ ÁLLAPOTOKAT
        bool inventoryOpen = container.activeInHierarchy;

        // Csak akkor engedjük az Inventory kezelését (nyitást/bezárást), ha:
        // - Már nyitva van (hogy be tudd zárni)
        // - VAGY a játék fut (Time.timeScale > 0), tehát nincs nyitva a Quest panel vagy más menü.
        if (inventoryOpen || Time.timeScale > 0)
        {
            HandleInventoryToggle();
        }

        bool isUIOpen = container.activeInHierarchy;

        bool canInteract = Time.timeScale > 0 || currentOpenedChest != null;

        if (canInteract)
        {
            PerformInteractionDetection();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (currentOpenedChest != null)
                {
                    currentOpenedChest.CloseChest();
                }
                else if (!isUIOpen)
                {
                    ExecuteInteraction();
                }
            }

            if (Time.timeScale > 0)
            {
                HandleHotbarSelection();
                HandleDropEquippedItem();
                HandleItemUsage();
            }
        }

        HandleDragLogic();
        HandleRightClickSplit();
    }

    private void HandleItemUsage()
    {
        if (container.activeInHierarchy || isDragging) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

            if (equippedSlot.HasItem())
            {
                InventoryItemSO item = equippedSlot.GetItem();
                bool consumed = item.Use();

                if (consumed)
                {
                    int remaining = equippedSlot.GetAmount() - 1;
                    if (remaining <= 0)
                    {
                        equippedSlot.ClearSlot();
                        EquipHandItem();
                    }
                    else
                    {
                        equippedSlot.SetItem(item, remaining);
                    }
                    NotifyQuestManagerOfInventoryChange();
                }
            }
        }
    }

    // --- MÓDOSÍTOTT ADDIITEM METÓDUS ---
    public void AddItem(InventoryItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        // 1. LÉPÉS: Meglévõ stackek keresése ELÕSZÖR a HOTBARON, aztán az Inventoryban
        foreach (Slot slot in allSlots) // Mivel az Awake-ben megfordítottuk a sorrendet, ez jó
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int spaceLeft = itemToAdd.maxStackSize - currentAmount;
                if (spaceLeft > 0)
                {
                    int amountToFill = Mathf.Min(spaceLeft, remaining);
                    slot.SetItem(itemToAdd, currentAmount + amountToFill);
                    remaining -= amountToFill;
                    if (remaining <= 0) break;
                }
            }
        }

        // 2. LÉPÉS: Ha még maradt tárgy, üres hely keresése ELÕSZÖR a HOTBARON
        if (remaining > 0)
        {
            // Elõször csak a hotbart nézzük végig üres helyért
            foreach (Slot slot in hotbarSlots)
            {
                if (!slot.HasItem())
                {
                    int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                    slot.SetItem(itemToAdd, amountToPlace);
                    remaining -= amountToPlace;
                    if (remaining <= 0) break;
                }
            }
        }

        // 3. LÉPÉS: Ha a hotbar betelt, mehet az Inventory üres helyeire
        if (remaining > 0)
        {
            foreach (Slot slot in inventorySlots)
            {
                if (!slot.HasItem())
                {
                    int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                    slot.SetItem(itemToAdd, amountToPlace);
                    remaining -= amountToPlace;
                    if (remaining <= 0) break;
                }
            }
        }

        // Frissítjük a kezünkben lévõ tárgyat, ha épp abba a slotba került valami, amit fogunk
        EquipHandItem();
    }

    private void PerformInteractionDetection()
    {
        ResetHighlight();
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ~excludeLayers))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                currentlyLookedAtItem = item;
                ApplyHighlight(item.GetComponent<Renderer>());
                return;
            }

            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null)
            {
                currentlyLookedAtChest = chest;
                ApplyHighlight(chest.GetComponent<Renderer>());
            }
        }
    }

    private void ApplyHighlight(Renderer rend)
    {
        if (rend == null) return;
        lookedAtRenderer = rend;
        originalMaterial = rend.material;
        rend.material = highlightMaterial;
    }

    private void ResetHighlight()
    {
        if (lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
        }
        currentlyLookedAtItem = null;
        currentlyLookedAtChest = null;
    }

    private void ExecuteInteraction()
    {
        if (currentlyLookedAtChest != null)
        {
            currentOpenedChest = currentlyLookedAtChest;
            currentOpenedChest.OpenChest();
        }
        else if (currentlyLookedAtItem != null)
        {
            PickupItem();
        }
    }

    public void RegisterExternalSlots(List<Slot> externalSlots)
    {
        foreach (var slot in externalSlots)
        {
            if (!allSlots.Contains(slot)) allSlots.Add(slot);
            slot.hovering = false;
        }
    }

    public void UnregisterExternalSlots(List<Slot> externalSlots)
    {
        foreach (var slot in externalSlots)
        {
            slot.hovering = false;
            allSlots.Remove(slot);
        }
        currentOpenedChest = null;
    }

    private void HandleInventoryToggle()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame || (currentOpenedChest != null && Keyboard.current.escapeKey.wasPressedThisFrame))
        {
            if (currentOpenedChest != null)
                currentOpenedChest.CloseChest();
            else
                ToggleInventoryUI(!container.activeInHierarchy);
        }
    }

    public void ToggleInventoryUI(bool isOpen)
    {
        container.SetActive(isOpen);
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
        Time.timeScale = isOpen ? 0f : 1f;
        if (hudCanvas != null) hudCanvas.SetActive(!isOpen);

        if (!isOpen)
        {
            foreach (var slot in allSlots) slot.hovering = false;
        }
    }

    private void PickupItem()
    {
        if (currentlyLookedAtItem != null)
        {
            AddItem(currentlyLookedAtItem.item, currentlyLookedAtItem.amount);
            NotifyQuestManagerOfInventoryChange();
            Destroy(currentlyLookedAtItem.gameObject);
            ResetHighlight();
            EquipHandItem();
        }
    }

    private void HandleHotbarSelection()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Keyboard.current[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame)
            {
                equippedHotbarIndex = i;
                UpdateHotbarOpacity();
                EquipHandItem();
                break;
            }
        }
    }

    private void UpdateHotbarOpacity()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            Image slotImage = hotbarSlots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                Color c = slotImage.color;
                c.a = (i == equippedHotbarIndex) ? equppedOpacity : normalOpacity;
                slotImage.color = c;
            }
        }
    }

    private void EquipHandItem()
    {
        if (currentHandItem != null) Destroy(currentHandItem);
        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
        if (!equippedSlot.HasItem() || equippedSlot.GetItem().handItemPrefab == null) return;

        currentHandItem = Instantiate(equippedSlot.GetItem().handItemPrefab, hand);
        currentHandItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void HandleDropEquippedItem()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
            if (equippedSlot.HasItem() && equippedSlot.GetItem().itemPrefab != null)
            {
                InventoryItemSO itemSO = equippedSlot.GetItem();
                Vector3 dropPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                GameObject dropped = Instantiate(itemSO.itemPrefab, dropPos, Quaternion.identity);

                Item itemComp = dropped.GetComponent<Item>();
                itemComp.item = itemSO;
                itemComp.amount = equippedSlot.GetAmount();

                equippedSlot.ClearSlot();
                EquipHandItem();
            }
        }
    }

    private void HandleDragLogic()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null && hovered.HasItem())
            {
                dragedSlot = hovered;
                isDragging = true;
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.enabled = true;
            }
        }

        if (isDragging)
        {
            dragIcon.transform.position = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Slot hovered = GetHoveredSlot();
                if (hovered != null) HandleDrop(dragedSlot, hovered);

                dragIcon.enabled = false;
                isDragging = false;
                dragedSlot = null;
            }
        }
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;
        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int move = Mathf.Min(to.GetItem().maxStackSize - to.GetAmount(), from.GetAmount());
            to.SetItem(to.GetItem(), to.GetAmount() + move);
            from.SetItem(from.GetItem(), from.GetAmount() - move);
            if (from.GetAmount() <= 0) from.ClearSlot();
        }
        else
        {
            InventoryItemSO tempItem = to.GetItem();
            int tempAmt = to.GetAmount();
            to.SetItem(from.GetItem(), from.GetAmount());
            if (tempItem != null) from.SetItem(tempItem, tempAmt);
            else from.ClearSlot();
        }
        EquipHandItem();
        NotifyQuestManagerOfInventoryChange();
    }

    private void HandleRightClickSplit()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null && hovered.HasItem() && hovered.GetAmount() > 1)
            {
                int half = hovered.GetAmount() / 2;
                foreach (Slot slot in allSlots)
                {
                    if (!slot.HasItem())
                    {
                        slot.SetItem(hovered.GetItem(), half);
                        hovered.SetItem(hovered.GetItem(), hovered.GetAmount() - half);
                        NotifyQuestManagerOfInventoryChange();
                        break;
                    }
                }
            }
        }
    }

    private Slot GetHoveredSlot()
    {
        return allSlots.Find(s => s.hovering && s.gameObject.activeInHierarchy);
    }

    public int GetItemQuantity(InventoryItemSO itemSO)
    {
        int total = 0;
        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemSO) total += slot.GetAmount();
        }
        return total;
    }

    private void NotifyQuestManagerOfInventoryChange()
    {
        if (QuestManager.Instance == null || QuestInputTracker.Instance == null) return; 
  

    foreach (QuestSO quest in QuestInputTracker.Instance.activeQuests)
        {
            foreach (QuestObjective obj in quest.questObjectives)
            {
                if (obj.targetItem != null)
                    QuestManager.Instance.UpdateObjectiveProgress(quest, obj);
            }
        }
    }
}

