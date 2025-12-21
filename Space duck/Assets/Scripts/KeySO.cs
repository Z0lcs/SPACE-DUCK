using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeySO", menuName = "Create SO/KeySO")]
public class KeySO : ScriptableObject
{
    public string actionName; 

    [Header("Key Group")]
    public List<KeyCode> requiredKeys;

    public bool IsAnyKeyPressed()
    {
        foreach (KeyCode k in requiredKeys)
        {
            if (Input.GetKey(k)) return true;
        }
        return false;
    }

    public bool AreAllKeysPressed()
    {
        foreach (KeyCode k in requiredKeys)
        {
            if (!Input.GetKey(k)) return false;
        }
        return true;
    }
}