using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    [System.Serializable]
    public struct LootSettings
    {
        public string megnevezes;
        public GameObject itemPrefab;
        public int hanyDobozba;
        public int darabPerDoboz;
    }

    public List<LootSettings> lootLista;
    public static LootManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(DistributeLootToBoxes());
    }

    IEnumerator DistributeLootToBoxes()
    {
        // Várunk egy kicsit, hogy minden doboz beregisztrálja magát
        yield return new WaitForSeconds(0.2f);

        List<Breakable> allBoxes = Breakable.GetAllBoxes();

        // Dobozok sorrendjének megkeverése
        for (int i = 0; i < allBoxes.Count; i++)
        {
            Breakable temp = allBoxes[i];
            int randomIndex = Random.Range(i, allBoxes.Count);
            allBoxes[i] = allBoxes[randomIndex];
            allBoxes[randomIndex] = temp;
        }

        int boxIndex = 0;
        foreach (var loot in lootLista)
        {
            for (int i = 0; i < loot.hanyDobozba; i++)
            {
                if (boxIndex < allBoxes.Count)
                {
                    allBoxes[boxIndex].SetLoot(loot.itemPrefab, loot.darabPerDoboz);
                    boxIndex++;
                }
            }
        }
        Debug.Log("Loot sikeresen szétosztva!");
    }
}
