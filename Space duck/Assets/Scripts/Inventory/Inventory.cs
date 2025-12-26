using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class Inventory : MonoBehaviour
{
    [Header("Items & Prefabs")]
    public InventoryItemSO woodItem;
    public InventoryItemSO axeItem;
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
    private GameObject currentHandItem;
    private int equippedHotbarIndex = 0;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot dragedSlot = null;
    private bool isDragging = false;

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);
    }

    private void Start()
    {
        UpdateHotbarOpacity();
        EquipHandItem();
    }

    void Update()
    {
        

        HandleInventoryToggle();

        if (Time.timeScale > 0)
        {
            DetectLookAtItem();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                PickupItem();
            }

            HandleHotbarSelection();
            HandleDropEquippedItem();
        }

        HandleDragLogic();
        HandleRightClickSplit();
    }

    public int GetItemQuantity(InventoryItemSO itemSO)
    {
        int total = 0;
        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemSO)
            {
                total += slot.GetAmount();
            }
        }
        return total;
    }

    private void HandleInventoryToggle()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            bool isInventoryOpen = !container.activeInHierarchy;
            container.SetActive(isInventoryOpen);

            Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isInventoryOpen;
            Time.timeScale = isInventoryOpen ? 0f : 1f;

            if (hudCanvas != null) hudCanvas.SetActive(!isInventoryOpen);
        }
    }

    private void DetectLookAtItem()
    {
        if (lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            currentlyLookedAtItem = null;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ~excludeLayers))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                currentlyLookedAtItem = item;
                Renderer rend = item.GetComponent<Renderer>();
                if (rend != null)
                {
                    originalMaterial = rend.material;
                    rend.material = highlightMaterial;
                    lookedAtRenderer = rend;
                }
            }
        }
    }

    private void PickupItem()
    {
        if (currentlyLookedAtItem != null)
        {
            AddItem(currentlyLookedAtItem.item, currentlyLookedAtItem.amount);
            lookedAtRenderer = null;
            Destroy(currentlyLookedAtItem.gameObject);
            currentlyLookedAtItem = null;
            EquipHandItem();
        }
    }

    public void AddItem(InventoryItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in allSlots)
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
                    if (remaining <= 0) return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;
                if (remaining <= 0) return;
            }
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
        if (!equippedSlot.HasItem()) return;

        InventoryItemSO item = equippedSlot.GetItem();
        if (item.handItemPrefab == null) return;

        currentHandItem = Instantiate(item.handItemPrefab, hand);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;
    }

    private void HandleDropEquippedItem()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
            if (equippedSlot.HasItem())
            {
                InventoryItemSO itemSO = equippedSlot.GetItem();
                if (itemSO.itemPrefab != null)
                {
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
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();
            int move = Mathf.Min(space, from.GetAmount());

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
                        break;
                    }
                }
            }
        }
    }

    private Slot GetHoveredSlot()
    {
        foreach (Slot s in allSlots) if (s.hovering) return s;
        return null;
    }
}