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
        // Elõzõ tárgy törlése a kézbõl
        if (currentHandItem != null) Destroy(currentHandItem);

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

        // Ha üres a slot, vagy nincs prefabja, nem rakunk semmit a kézbe
        if (!equippedSlot.HasItem() || equippedSlot.GetItem().handItemPrefab == null) return;

        // Új tárgy létrehozása a 'hand' Transform alatt
        currentHandItem = Instantiate(equippedSlot.GetItem().handItemPrefab, hand);
        currentHandItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
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