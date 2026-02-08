using UnityEngine;
using System.Collections.Generic;

public class Smelter : MonoBehaviour
{
    [Header("Slots")]
    public Slot inputSlot;
    public Slot fuelSlot;
    public Slot outputSlot;

    [HideInInspector]
    public List<Slot> smelterSlots = new List<Slot>();

    [Header("Minigame State")]
    public SmelterSession currentSession;

    private void Awake()
    {
        // Biztosítjuk, hogy a lista fel legyen töltve
        smelterSlots.Clear();
        if (inputSlot != null) smelterSlots.Add(inputSlot);
        if (fuelSlot != null) smelterSlots.Add(fuelSlot);
        if (outputSlot != null) smelterSlots.Add(outputSlot);
    }

    public void OpenSmelter()
    {
        // 1. Regisztráljuk a slotokat az Inventory rendszerbe, hogy lehessen beléjük pakolni
        Inventory.Instance.RegisterExternalSlots(smelterSlots);

        // 2. KÉNYSZERÍTETT FRISSÍTÉS: Megmondjuk a slotoknak, hogy rajzolják ki magukat
        // Ez oldja meg, hogy ne üres négyzeteket láss
        foreach (Slot slot in smelterSlots)
        {
            slot.UpdateSlot();
        }

        // 3. Ha van benne érc, de nem megy a gép, indítsuk el
        if (inputSlot.HasItem() && currentSession == null)
        {
            currentSession = new SmelterSession(inputSlot.GetItem());
        }
    }

    //private void Update()
    //{
        // Folyamat indítása/leállítása az input slot tartalma alapján
        //if (inputSlot.HasItem() && currentSession == null)
        //{
        //    currentSession = new SmelterSession(inputSlot.GetItem());
        //}

    //    if (!inputSlot.HasItem() && currentSession != null)
    //    {
    //        currentSession = null;
    //    }

    //    // Ha fut a folyamat, Tick és állapotellenõrzés
    //    if (currentSession != null)
    //    {
    //        currentSession.Tick(Time.deltaTime);

    //        if (currentSession.isExploded) ExplodeSmelter();
    //        else if (currentSession.isFinished) FinishProcess();
    //    }
    //}

    private void FinishProcess()
    {
        if (inputSlot.GetItem() == null) return;

        InventoryItemSO result = inputSlot.GetItem().smeltedResult;
        inputSlot.RemoveAmount(1);

        if (outputSlot.HasItem())
        {
            outputSlot.AddAmount(1);
        }
        else
        {
            outputSlot.SetItem(result, 1);
        }

        Inventory.Instance.NotifyQuestManagerOfInventoryChange();
        currentSession = null; // Reset a következõ tárgyhoz
    }

    private void ExplodeSmelter()
    {
        Debug.LogError("BUMM! A kemence felrobbant a túlnyomástól!");
        currentSession = null;
        // Itt spawolhatsz robbanás effektet: Instantiate(explosionPrefab, transform.position, ...)
        Destroy(gameObject);
    }
}