using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Audio;

public class Breakable : MonoBehaviour
{
    [Header("Grafika és Effektek")]
    public GameObject debrisPrefab;
    public AudioClip breakSound;
    public AudioMixerGroup outputGroup;

    [Header("Fizika")]
    public float breakForce = 500f;
    public float breakRange = 10f;
    public LayerMask layerMask;

    private GameObject _itemToSpawn;
    private int _countToSpawn;
    private bool _isBroken = false;

    private static List<Breakable> _activeBoxes = new List<Breakable>();
    private static int _globalHitCount = 0;

    void Awake() => _activeBoxes.Add(this);
    void OnDestroy() => _activeBoxes.Remove(this);

    public static List<Breakable> GetAllBoxes() => new List<Breakable>(_activeBoxes);

    public void SetLoot(GameObject item, int count)
    {
        _itemToSpawn = item;
        _countToSpawn = count;
    }

    void Update()
    {
        if (_isBroken) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, breakRange, layerMask))
            {
                if (hit.collider.gameObject == this.gameObject || hit.transform.IsChildOf(this.transform))
                {
                    Explode();
                }
            }
        }
    }

    public void Explode()
    {
        if (_isBroken) return;
        _isBroken = true;

        _globalHitCount++;
        if (_globalHitCount >= 5)
        {
            if (HungerManager.Instance != null)
            {
                HungerManager.Instance.DecreaseHunger(1);
            }
            _globalHitCount = 0;
        }

        if (breakSound != null)
        {
            GameObject tempGO = new GameObject("BreakSoundTemp");
            tempGO.transform.position = transform.position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();

            aSource.clip = breakSound;

            if (outputGroup != null)
            {
                aSource.outputAudioMixerGroup = outputGroup;
            }

            aSource.Play();

            Destroy(tempGO, breakSound.length);
        }

        if (debrisPrefab != null)
        {
            GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
            foreach (Rigidbody rb in debris.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(breakForce, transform.position, 2f);
            }
            Destroy(debris, 5f);
        }

        if (_itemToSpawn != null)
        {
            for (int i = 0; i < _countToSpawn; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * 0.8f;
                Vector3 spawnOffset = new Vector3(randomCircle.x, 0.5f, randomCircle.y);
                Vector3 targetPos = transform.position + spawnOffset;

                if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, out RaycastHit hit, 5f, layerMask))
                {
                    targetPos = hit.point + new Vector3(0, 0.1f, 0); 
                }

                GameObject item = Instantiate(_itemToSpawn, targetPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

                if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    Vector3 pushDir = new Vector3(randomCircle.x, 2f, randomCircle.y);
                    rb.AddForce(pushDir, ForceMode.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }
}