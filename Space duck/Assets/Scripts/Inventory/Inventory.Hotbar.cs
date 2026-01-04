using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class Inventory
{
    private void HandleHotbarSelection()
    {
        // 1-6 gombok figyelése
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
                // A kiválasztott slot kapja az 'equipped' átlátszóságot, a többi a 'normal'-t
                c.a = (i == equippedHotbarIndex) ? equppedOpacity : normalOpacity;
                slotImage.color = c;
            }
        }
    }

    private void EquipHandItem()
    {
        if (currentHandItem != null) Destroy(currentHandItem);

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

        // Ellenõrzés
        if (!equippedSlot.HasItem()) return;

        // Itt a titok: HASZNÁLD AZ InventoryItemSO típust az Item helyett!
        InventoryItemSO itemData = equippedSlot.GetItem();

        if (itemData.handItemPrefab == null) return;

        // 1. Példányosítás
        currentHandItem = Instantiate(itemData.handItemPrefab, hand);

        // 2. Beállítás az SO-ból
        // FIGYELEM: Ha az InventoryItemSO-ban még nincsenek benne ezek a változók, 
        // akkor add hozzá õket az InventoryItemSO scriptben!
        currentHandItem.transform.localPosition = itemData.handPositionOffset;
        currentHandItem.transform.localEulerAngles = itemData.handRotationOffset;
    }

    private void HandleDropEquippedItem()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
            if (equippedSlot.HasItem() && equippedSlot.GetItem().itemPrefab != null)
            {
                InventoryItemSO itemSO = equippedSlot.GetItem();

                // Kidobás a kamera elé
                Vector3 dropPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                GameObject dropped = Instantiate(itemSO.itemPrefab, dropPos, Quaternion.identity);

                // Adatok átadása a kidobott objektumnak
                Item itemComp = dropped.GetComponent<Item>();
                itemComp.item = itemSO;
                itemComp.amount = equippedSlot.GetAmount();

                equippedSlot.ClearSlot();
                EquipHandItem();
                NotifyQuestManagerOfInventoryChange();
            }
        }
    }
}