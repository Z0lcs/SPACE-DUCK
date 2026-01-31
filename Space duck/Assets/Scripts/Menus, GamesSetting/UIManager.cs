using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // Singleton minta, hogy bárhonnan elérd: UIManager.Instance
    public static UIManager Instance { get; private set; }

    [Header("Referenciák")]
    public CraftingTable craftingTable;
    public List<GameObject> otherPanels;

    private void Awake()
    {
        // Beállítjuk a Singletont
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Ha megnyomják az ESC-et, ugyanazt hívjuk meg
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAll();
        }
    }

    // EZT A FÜGGVÉNYT TUDOD MEGHÍVNI MÁS KÓDBÓL
    public void CloseAll()
    {
        // 1. Speciális bezárás a CraftingTable-nek (hogy a tárgyak megmaradjanak)
        if (craftingTable != null && craftingTable.craftingPanel.activeSelf)
        {
            craftingTable.CloseTable();
        }

        // 2. Minden más panel kikapcsolása
        foreach (GameObject panel in otherPanels)
        {
            if (panel != null) panel.SetActive(false);
        }

        // 3. Kurzor alaphelyzetbe állítása
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}