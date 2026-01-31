using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HungerManager : MonoBehaviour
{
    // Singleton minta, hogy bárhonnan könnyen elérd: HungerManager.Instance
    public static HungerManager Instance;

    private List<Image> pizzaIcons = new List<Image>();
    public int maxHunger;
    public int currentHunger;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Összeszedi az összes Image-et, ami a gyerek-objektuma
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null) pizzaIcons.Add(img);
        }

        maxHunger = pizzaIcons.Count;
        currentHunger = maxHunger;
        UpdateUI();
    }

    public void DecreaseHunger(int amount)
    {
        currentHunger -= amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateUI();
    }

    public void IncreaseHunger(int amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < pizzaIcons.Count; i++)
        {
            // Bekapcsolja a pizzát ha még van annyi éhségpont, egyébként kikapcsolja
            pizzaIcons[i].enabled = i < currentHunger;
        }
    }
}