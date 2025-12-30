using UnityEngine;
using UnityEngine.InputSystem;

public partial class Inventory
{
    private void HandleInteractionInput(bool isUIOpen)
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentOpenedChest != null)
                currentOpenedChest.CloseChest();
            else if (!isUIOpen)
                ExecuteInteraction();
        }
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
}