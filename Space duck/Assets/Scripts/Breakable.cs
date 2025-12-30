using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private float _breakForce = 5;
    [SerializeField] private float _collisionMultiplier = 100;
    [SerializeField] private float _breakRange = 5f;

    [Header("Cleanup")]
    [Tooltip("Ha be van pipálva, a törmelék sosem tűnik el.")]
    [SerializeField] private bool neverDestroyPieces = false;
    [SerializeField] private float duration = 5f;
    [Tooltip("Hány csoport/darab jöjjön létre a széttöréskor.")]
    [SerializeField] private int amount = 1;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip boxExplosionSound;

    [Header("Break Delay")]
    public float visualBreakDelay = 1.0f;

    private bool _broken;
    private static int totalHitCount = 0;

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

                    totalHitCount++;

                    if (totalHitCount >= 5)
                    {
                        if (HungerManager.Instance != null)
                        {
                            HungerManager.Instance.DecreaseHunger(1);
                        }
                        totalHitCount = 0;
                    }

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

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnOffset = (amount > 1) ? new Vector3(Random.Range(-4.0f, 4.0f), 0, Random.Range(-4.0f, 4.0f)) : Vector3.zero;

            GameObject replacement = Instantiate(_replacement, transform.position + spawnOffset, Quaternion.identity);

            if (!neverDestroyPieces)
            {
                Destroy(replacement, duration);
            }

            var rbs = replacement.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                rb.AddExplosionForce(_breakForce * _collisionMultiplier, impactPoint, 10);

                Vector3 randomScatter = new Vector3(Random.Range(-1.5f, 1.5f), 0.1f, Random.Range(-1.5f, 1.5f));
                rb.AddForce(randomScatter * _collisionMultiplier, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}