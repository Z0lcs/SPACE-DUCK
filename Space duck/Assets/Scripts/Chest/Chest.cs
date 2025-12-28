using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    [Header("Chest UI")]
    public GameObject chestUIPanel;
    public Transform chestSlotParent;

    [Header("Settings")]
    public float closeDistance = 4f;

    private List<Slot> chestSlots = new List<Slot>();
    private bool isChestOpen = false;
    private Transform playerTransform;
    private float sqrCloseDistance;

    private void Awake()
    {
        // Inicializáljuk a listát és összegyûjtjük a slotokat
        chestSlots = new List<Slot>();

        if (chestSlotParent != null)
        {
            Slot[] slots = chestSlotParent.GetComponentsInChildren<Slot>();
            if (slots != null) chestSlots.AddRange(slots);
        }

        sqrCloseDistance = closeDistance * closeDistance;
    }

    private void Start()
    {
        // Játékos megkeresése a távolság alapú bezáráshoz
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        // Automatikus bezárás, ha a játékos túl messzire megy
        if (isChestOpen && playerTransform != null)
        {
            float sqrDist = (transform.position - playerTransform.position).sqrMagnitude;
            if (sqrDist > sqrCloseDistance)
            {
                CloseChest();
            }
        }
    }

    public void OpenChest()
    {
        isChestOpen = true;
        if (chestUIPanel != null) chestUIPanel.SetActive(true);

        if (Inventory.Instance != null)
        {
            Inventory.Instance.ToggleInventoryUI(true);
            Inventory.Instance.RegisterExternalSlots(chestSlots);
        }
    }

    public void CloseChest()
    {
        if (!isChestOpen) return;

        isChestOpen = false;
        if (chestUIPanel != null) chestUIPanel.SetActive(false);

        if (Inventory.Instance != null)
        {
            Inventory.Instance.UnregisterExternalSlots(chestSlots);
            Inventory.Instance.ToggleInventoryUI(false);
        }
    }

    /// <summary>
    /// Megszámolja, hogy egy adott tárgyból mennyi van összesen a ládában.
    /// </summary>
    public int GetItemQuantityInChest(InventoryItemSO itemSO)
    {
        if (itemSO == null || chestSlots == null) return 0;

        int total = 0;
        foreach (Slot slot in chestSlots)
        {
            if (slot != null && slot.HasItem() && slot.GetItem() == itemSO)
            {
                total += slot.GetAmount();
            }
        }
        return total;
    }
}