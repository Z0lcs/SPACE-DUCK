using UnityEngine;
using UnityEngine.InputSystem; // Az új Input System miatt
using UnityEngine.UI;

public class Mining : MonoBehaviour
{
    [Header("Beállítások")]
    public float miningRange = 4f;       // A te pickupRange-ednek felel meg
    public LayerMask excludeLayers;      // Rétegek, amiket ne találjon el
    public Slider powerSlider;           // A UI csúszka
    public float sliderSpeed = 2.5f;

    private bool isSettingPower = false;
    private Ore currentlyLookedAtOre;    // Mint a currentlyLookedAtItem
    private bool hitBonusPoint = false;

    void Start()
    {
        if (powerSlider != null) powerSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isSettingPower)
        {
            HandlePowerSlider();
        }
        else
        {
            // Folyamatosan nézzük, van-e elõttünk érc (mint a PerformInteractionDetection)
            PerformOreDetection();

            // Kattintásra indul a bányászat (mint a HandleInteractionInput)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (currentlyLookedAtOre != null)
                {
                    StartMiningPowerSequence();
                }
            }
        }
    }

    private void PerformOreDetection()
    {
        currentlyLookedAtOre = null;
        hitBonusPoint = false;

        // PONTOSAN A TE RAYCAST LOGIKÁD:
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, miningRange, ~excludeLayers))
        {
            // Megnézzük, hogy érc-e vagy a rajta lévõ bónusz pont (HitMarker)
            Ore ore = hit.collider.GetComponentInParent<Ore>();

            if (ore != null)
            {
                currentlyLookedAtOre = ore;

                // Ha a HitMarker taget találtuk el, az bónusz
                if (hit.collider.CompareTag("HitMarker"))
                {
                    hitBonusPoint = true;
                }
            }
        }
    }

    private void StartMiningPowerSequence()
    {
        // Karakter odafordítása az érchez (TPP-nél fontos)
        Vector3 lookTarget = currentlyLookedAtOre.transform.position;
        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);

        isSettingPower = true;
        powerSlider.gameObject.SetActive(true);
    }

    private void HandlePowerSlider()
    {
        powerSlider.value = Mathf.PingPong(Time.time * sliderSpeed, 1f);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            FinalizeMiningHit();
        }
    }

    private void FinalizeMiningHit()
    {
        float power = powerSlider.value;
        if (power > 0.9f) power *= 2f; // Tökéletes idõzítés bónusz

        if (currentlyLookedAtOre != null)
        {
            currentlyLookedAtOre.GetHit(power, hitBonusPoint);
        }

        // Alaphelyzetbe állítás
        isSettingPower = false;
        powerSlider.gameObject.SetActive(false);
    }
}