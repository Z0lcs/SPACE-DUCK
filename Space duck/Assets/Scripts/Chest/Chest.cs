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
    private float sqrCloseDistance; // Gyorsítótárazott négyzetes távolság

    private void Awake()
    {
        if (chestSlotParent != null)
        {
            chestSlots.AddRange(chestSlotParent.GetComponentsInChildren<Slot>());
        }
        // Elõre kiszámoljuk a távolság négyzetét
        sqrCloseDistance = closeDistance * closeDistance;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (isChestOpen && playerTransform != null)
        {
            // sqrMagnitude sokkal gyorsabb az Update-ben
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

        Inventory.Instance.ToggleInventoryUI(true);
        // Csak regisztráljuk a már Awake-ben kigyûjtött slotokat
        Inventory.Instance.RegisterExternalSlots(chestSlots);
    }

    public void CloseChest()
    {
        if (!isChestOpen) return;

        isChestOpen = false;
        if (chestUIPanel != null) chestUIPanel.SetActive(false);

        Inventory.Instance.UnregisterExternalSlots(chestSlots);
        Inventory.Instance.ToggleInventoryUI(false);
    }
}