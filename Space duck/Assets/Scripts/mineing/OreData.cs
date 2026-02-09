using UnityEngine;

[CreateAssetMenu(fileName = "UjErc", menuName = "Mining/OreData")]
public class OreData : ScriptableObject
{
    public string oreName;
    public float maxHealth;
    public Color markerColor; // A kis célpont színe (pl. az aranynál sárga)
    public int yieldAmount;   // Mennyit kap a játékos
}