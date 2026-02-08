using UnityEngine;

[System.Serializable]
public class SmelterSession
{
    public float temp = 20f;
    public float pressure = 0f;
    public float progress = 0f;
    public float zoneTimer = 0f;
    public float penaltyTime = 0f;

    public bool isExploded = false;
    public bool isFinished = false;

    private InventoryItemSO activeItem;

    public SmelterSession(InventoryItemSO item)
    {
        activeItem = item;
    }

    public void Tick(float deltaTime)
    {
        // Ha nincs tárgy (biztonsági mentés), ne csináljon semmit
        if (activeItem == null) return;

        // Alap nyomásnövekedés és hûlés
        pressure += 1.5f * deltaTime;
        temp = Mathf.MoveTowards(temp, 20f, 2f * deltaTime);

        // Zónák ellenõrzése
        if (temp > 800)
        {
            pressure += 5f * deltaTime;
            zoneTimer += deltaTime;
            if (zoneTimer >= 5f) isExploded = true;
        }
        else if (temp < 400)
        {
            zoneTimer += deltaTime;
            if (zoneTimer >= 5f)
            {
                penaltyTime += 15f;
                zoneTimer = 0f;
            }
        }
        else
        {
            zoneTimer = 0f;
        }

        // Haladás (51. sor javítva: activeItem ellenõrzése után fut)
        progress += deltaTime;
        if (progress >= (activeItem.smeltingTime + penaltyTime))
        {
            isFinished = true;
        }

        if (pressure >= 100f) isExploded = true;
    }

    public void AddHeat(float amount) => temp += amount;
    public void CoolDown()
    {
        temp -= 60f;
        pressure += 15f;
    }
}