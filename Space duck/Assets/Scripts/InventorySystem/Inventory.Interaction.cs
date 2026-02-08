using UnityEngine;
using UnityEngine.InputSystem;

public partial class Inventory
{
    private Material[] originalMaterials;

    // Új változók a kemence kezeléséhez
    private Smelter currentlyLookedAtSmelter;

    private void HandleInteractionInput(bool isUIOpen)
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Ha nyitva van egy láda, zárjuk be
            if (currentOpenedChest != null)
            {
                currentOpenedChest.CloseChest();
            }
            // Ha nincs nyitva az UI, nézzük meg, mire kattintottunk
            else if (!isUIOpen)
            {
                ExecuteInteraction();
            }
        }
    }

    private void PerformInteractionDetection()
    {
        ResetHighlight();
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ~excludeLayers))
        {
            // 1. Tárgy felismerése
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                currentlyLookedAtItem = item;
                ApplyHighlight(item.GetComponent<Renderer>());
                return;
            }

            // 2. Láda felismerése
            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null)
            {
                currentlyLookedAtChest = chest;
                ApplyHighlight(chest.GetComponent<Renderer>());
                return;
            }

            // 3. Kemence felismerése
            Smelter smelter = hit.collider.GetComponent<Smelter>();
            if (smelter != null)
            {
                currentlyLookedAtSmelter = smelter;
                ApplyHighlight(smelter.GetComponent<Renderer>());
            }
        }
    }

    private void ExecuteInteraction()
    {
        if (currentlyLookedAtItem != null)
        {
            PickupItem();
        }
        else if (currentlyLookedAtChest != null)
        {
            currentlyLookedAtChest.OpenChest();
        }
        else if (currentlyLookedAtSmelter != null)
        {
            // Megnyitjuk a kemence logikáját
            currentlyLookedAtSmelter.OpenSmelter();

            // Bekapcsoljuk a Smelter UI-t
            SmelterUI ui = FindObjectOfType<SmelterUI>(true);
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
                ui.targetSmelter = currentlyLookedAtSmelter;
            }

            // Megnyitjuk az Inventory-t is mellé
            ToggleInventoryUI(true);
        }
    }

    private void ApplyHighlight(Renderer rend)
    {
        if (rend == null || lookedAtRenderer == rend) return;

        lookedAtRenderer = rend;
        originalMaterials = rend.sharedMaterials;

        Material[] tempMats = new Material[originalMaterials.Length];
        for (int i = 0; i < tempMats.Length; i++)
        {
            tempMats[i] = highlightMaterial;
        }

        rend.materials = tempMats;
    }

    private void ResetHighlight()
    {
        if (lookedAtRenderer != null && originalMaterials != null)
        {
            lookedAtRenderer.materials = originalMaterials;
            lookedAtRenderer = null;
            originalMaterials = null;
        }
        currentlyLookedAtItem = null;
        currentlyLookedAtChest = null;
        currentlyLookedAtSmelter = null; // Ezt is reseteljük!
    }

    private void PickupItem()
    {
        if (currentlyLookedAtItem != null)
        {
            AddItem(currentlyLookedAtItem.item, currentlyLookedAtItem.amount);
            Destroy(currentlyLookedAtItem.gameObject);
        }
    }
}