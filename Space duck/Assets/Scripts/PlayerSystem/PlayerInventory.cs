using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("Doboz felrobbantva!");
        }
    }
}