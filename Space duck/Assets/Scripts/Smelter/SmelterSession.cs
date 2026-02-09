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
        if (activeItem == null) return;

        // FOLYAMATOS VÁLTOZÁSOK
        pressure += 2.0f * deltaTime; // Nyomás magától nõ
        temp = Mathf.MoveTowards(temp, 20f, 4f * deltaTime); // Hõmérséklet magától csökken (hûl)

        // ZÓNÁK ELLENÕRZÉSE
        if (temp > 800) // Túl forró: Gyors nyomás és robbanás veszély
        {
            pressure += 8f * deltaTime;
            zoneTimer += deltaTime;
            if (zoneTimer >= 5f) isExploded = true;
        }
        else if (temp < 400) // Túl hideg: Nincs haladás, büntetõidõ nõ
        {
            zoneTimer += deltaTime;
            if (zoneTimer >= 3f)
            {
                penaltyTime += 5f;
                zoneTimer = 0f;
            }
        }
        else // IDEÁLIS ZÓNA (400-800 fok): Itt ég ki az érc!
        {
            zoneTimer = 0f;
            progress += deltaTime;
        }

        // BEFEJEZÉS ELLENÕRZÉSE
        if (progress >= (activeItem.smeltingTime + penaltyTime))
        {
            isFinished = true;
        }

        if (pressure >= 100f) isExploded = true;
    }

    public void AddHeat(float amount) => temp += amount;
    public void CoolDown() { temp -= 60f; pressure += 10f; }
}