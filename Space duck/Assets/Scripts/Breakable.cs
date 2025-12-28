using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private float _breakForce = 5;
    [SerializeField] private float _collisionMultiplier = 100;
    [SerializeField] private float _breakRange = 5f;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip boxExplosionSound;

    [Header("Break Delay")]
    public float visualBreakDelay = 1.0f;

    private bool _broken;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !_broken && Time.timeScale > 0)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit[] hits = Physics.RaycastAll(ray, _breakRange);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == transform)
                {
                    _broken = true;
                    StartCoroutine(BreakSequence(hit.point));
                    break;
                }
            }
        }
    }

    private IEnumerator BreakSequence(Vector3 impactPoint)
    {
        if (playerAudioSource != null && boxExplosionSound != null)
        {
            playerAudioSource.PlayOneShot(boxExplosionSound);
        }

        yield return new WaitForSeconds(visualBreakDelay);

        if (TryGetComponent<Renderer>(out var rend)) rend.enabled = false;
        if (TryGetComponent<Collider>(out var coll)) coll.enabled = false;

        // Szilánkok létrehozása
        GameObject replacement = Instantiate(_replacement, transform.position, transform.rotation);

        // --- ÚJ RÉSZ: A szilánkok (darabok) törlése 5 másodperc múlva ---
        Destroy(replacement, 5f);

        var rbs = replacement.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.AddExplosionForce(_breakForce * _collisionMultiplier, impactPoint, 2);

            Vector3 randomScatter = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 2f), Random.Range(-1f, 1f));
            rb.AddForce(randomScatter * _collisionMultiplier * 0.5f, ForceMode.Impulse);
        }

        // Az eredeti (most már láthatatlan) doboz törlése
        Destroy(gameObject);
    }
}