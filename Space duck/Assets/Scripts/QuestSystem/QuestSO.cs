using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    //public int questLevel;

    public List<QuestObjective> questObjectives;
}

[System.Serializable]
public class QuestObjective
{
    public string description;

    [SerializeField] private Object target;

    public InventoryItemSO targetItem => target as InventoryItemSO;
    public ActorSO targetNPC=> target as ActorSO;
    public LocationSO targetLocation => target as LocationSO;
    public KeySO targetKey => target as KeySO;

    public int requiredAmount;
}