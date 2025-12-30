using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class Inventory
{
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

    private Slot GetHoveredSlot()
    {
        return allSlots.Find(s => s.hovering && s.gameObject.activeInHierarchy);
    }
}