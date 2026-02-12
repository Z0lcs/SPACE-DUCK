using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _canOpen = false;
    private bool _isOpened = false;

    [Header("UI Beállítások")]
    public GameObject interactionText;

    [Header("Hang Beállítások")]
    public AudioClip openSound;
    public AudioClip closeSound;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (interactionText != null)
            interactionText.SetActive(false);

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (_canOpen && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (_animator != null)
        {
            _isOpened = !_isOpened;
            _animator.SetBool("isOpen", _isOpened);

            PlayDoorSound();
        }
    }

    private void PlayDoorSound()
    {
        if (_audioSource == null) return;

        AudioClip clipToPlay = _isOpened ? openSound : closeSound;

        if (clipToPlay != null)
        {
            _audioSource.clip = clipToPlay;
            _audioSource.Play();
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