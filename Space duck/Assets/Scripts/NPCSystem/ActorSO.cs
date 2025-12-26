using UnityEngine;

[CreateAssetMenu(fileName = "ActorSO", menuName = "Create SO/New NPC")]
public class ActorSO : ScriptableObject
{
    public string actorName;
    public Sprite portrait;
}
