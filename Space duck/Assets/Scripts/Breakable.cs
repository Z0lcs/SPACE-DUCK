using UnityEngine;
using System.Collections; 

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private float _breakForce = 2;
    [SerializeField] private float _collisionMultiplier = 100;
    [SerializeField] private bool _broken;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip boxExplosionSound;

    [Header("Break Delay")]
    public float visualBreakDelay = 1.0f; 

    void OnCollisionEnter(Collision collision)
    {
        if (_broken) return;

        if (collision.relativeVelocity.magnitude >= _breakForce)
        {
            _broken = true;

            if (playerAudioSource != null && boxExplosionSound != null)
            {
                playerAudioSource.PlayOneShot(boxExplosionSound);
            }

            StartCoroutine(BreakSequence(collision));
        }
    }
    private IEnumerator BreakSequence(Collision collision)
    {
        yield return new WaitForSeconds(visualBreakDelay);

        var replacement = Instantiate(_replacement, transform.position, transform.rotation);

        var rbs = replacement.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in rbs)
        {
            rb.AddExplosionForce(collision.relativeVelocity.magnitude * _collisionMultiplier, collision.contacts[0].point, 2);
        }

        Destroy(gameObject);
    }
}
