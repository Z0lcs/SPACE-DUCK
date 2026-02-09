using UnityEngine;
using System.Collections.Generic;

public class Smelter : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject smelterCanvas;

    public Slot inputSlot;
    public Slot fuelSlot;
    public Slot outputSlot;

    [HideInInspector]
    public List<Slot> smelterSlots = new List<Slot>();
    public SmelterSession currentSession;

    private void Awake()
    {
        smelterSlots.Clear();
        if (inputSlot != null) smelterSlots.Add(inputSlot);
        if (fuelSlot != null) smelterSlots.Add(fuelSlot);
        if (outputSlot != null) smelterSlots.Add(outputSlot);
    }

    private void Update() // EZT AKTIVÁLTAM
    {
        if (inputSlot.HasItem() && currentSession == null)
        {
            currentSession = new SmelterSession(inputSlot.GetItem());
        }

        if (currentSession != null)
        {
            currentSession.Tick(Time.deltaTime);

            if (currentSession.isExploded) ExplodeSmelter();
            else if (currentSession.isFinished) FinishProcess();
        }
    }

    public void OpenSmelter()
    {
        if (smelterCanvas != null) smelterCanvas.SetActive(true);
        Inventory.Instance.RegisterExternalSlots(smelterSlots);
        foreach (Slot slot in smelterSlots) slot.UpdateSlot();
    }

    public void EmergencyShutdown() // Vészmegszakító gomb hívja
    {
        if (currentSession != null)
        {
            inputSlot.RemoveAmount(inputSlot.GetAmount()); // Tárgyak törlése
            currentSession = null;
            Debug.Log("Vészleállítás! Tárgyak elvesztek.");
        }
    }

    private void FinishProcess()
    {
        if (inputSlot.GetItem() == null) return;

        InventoryItemSO result = inputSlot.GetItem().smeltedResult;
        inputSlot.RemoveAmount(1);

        if (outputSlot.HasItem()) outputSlot.AddAmount(1);
        else outputSlot.SetItem(result, 1);

        currentSession = null;
    }

    private void ExplodeSmelter()
    {
        inputSlot.RemoveAmount(inputSlot.GetAmount());
        currentSession = null;
        Destroy(gameObject);
    }
}