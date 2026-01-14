using UnityEngine;
using TMPro; 
public class DoorController : MonoBehaviour
{
    private Animator _animator;
    private bool _canOpen = false;
    private bool _isOpened = false;

    [Header("UI Beállítások")]
    public GameObject interactionText; 

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    void Update()
    {
        if (_canOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (_animator != null)
            {
                _isOpened = !_isOpened;
                _animator.SetBool("isOpen", _isOpened);

                // interactionText.SetActive(false); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canOpen = true;
            if (interactionText != null)
                interactionText.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canOpen = false;
            if (interactionText != null)
                interactionText.SetActive(false);
        }
    }
}