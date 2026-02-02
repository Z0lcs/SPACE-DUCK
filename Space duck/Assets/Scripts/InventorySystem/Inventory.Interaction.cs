using UnityEngine;
using UnityEngine.InputSystem;

public partial class Inventory
{
    // A változóid maradnak, de az originalMaterial-nak tömbnek kell lennie, 
    // különben fizikailag képtelen elmenteni a villáskulcs mindkét részét.
    
    private Material[] originalMaterials;

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
        if (rend == null || lookedAtRenderer == rend) return;

        lookedAtRenderer = rend;
        // sharedMaterials-t mentünk, hogy az összes réteg meglegyen
        originalMaterials = rend.sharedMaterials;

        // Létrehozunk egy tömböt, ami minden slotot kitölt a highlight-tal
        Material[] tempMats = new Material[originalMaterials.Length];
        for (int i = 0; i < tempMats.Length; i++)
        {
            tempMats[i] = highlightMaterial;
        }

        // Ez színezi át az EGÉSZET
        rend.materials = tempMats;
    }

    private void ResetHighlight()
    {
        // A NullReferenceException azért van, mert olyankor is akarsz resetelni, 
        // amikor nem is nézel semmire. Ez az IF megállítja a hibát:
        if (lookedAtRenderer != null && originalMaterials != null)
        {
            lookedAtRenderer.materials = originalMaterials;
            lookedAtRenderer = null;
            originalMaterials = null;
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

