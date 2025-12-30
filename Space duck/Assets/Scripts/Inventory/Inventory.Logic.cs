using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Inventory
{
    public void AddItem(InventoryItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        // 1. Stackelés meglévõ helyre (allSlots sorrendje miatt Hotbar az elsõ)
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
                    if (remaining <= 0) break;
                }
            }
        }

        // 2. Új slot keresése (Hotbar -> Inventory)
        if (remaining > 0)
        {
            foreach (Slot slot in allSlots) // Az allSlots már tartalmazza mindkettõt a jó sorrendben
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
        EquipHandItem();
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
                if (item.Use())
                {
                    int remaining = equippedSlot.GetAmount() - 1;
                    if (remaining <= 0) equippedSlot.ClearSlot();
                    else equippedSlot.SetItem(item, remaining);

                    EquipHandItem();
                    NotifyQuestManagerOfInventoryChange();
                }
            }
        }
    }

    private void HandleRightClickSplit()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && !isDragging)
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

    private void NotifyQuestManagerOfInventoryChange()
    {
        if (QuestManager.Instance == null || QuestInputTracker.Instance == null) return;

        foreach (var quest in QuestInputTracker.Instance.activeQuests)
        {
            foreach (var obj in quest.questObjectives)
            {
                if (obj.targetItem != null)
                    QuestManager.Instance.UpdateObjectiveProgress(quest, obj);
            }
        }
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

    // Szintén ellenõrizd, hogy ezek a metódusok is ott vannak-e a Logic fájlban, 
    // mert a Quest rendszernek szüksége van rájuk:
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
}