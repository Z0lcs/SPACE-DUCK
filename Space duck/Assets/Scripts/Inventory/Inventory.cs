using UnityEngine;
using System.Collections.Generic;
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

    // Privát változók a logikához
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
        bool inventoryOpen = container.activeInHierarchy;

        if (inventoryOpen || Time.timeScale > 0)
        {
            HandleInventoryToggle();
        }

        bool isUIOpen = container.activeInHierarchy;
        bool canInteract = Time.timeScale > 0 || currentOpenedChest != null;

        if (canInteract)
        {
            PerformInteractionDetection();
            HandleInteractionInput(isUIOpen);

            if (Time.timeScale > 0)
            {
                HandleHotbarSelection();   // Most már az Inventory.Hotbar.cs-ben
                HandleDropEquippedItem(); // Most már az Inventory.Hotbar.cs-ben
                HandleItemUsage();         // Most már az Inventory.Logic.cs-ben
            }
        }

        HandleDragLogic();        // Most már az Inventory.UI.cs-ben
        HandleRightClickSplit();  // Most már az Inventory.Logic.cs-ben
    }
}