using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Mining : MonoBehaviour
{
    [Header("Beállítások")]
    public float miningRange = 20f;

    // 1. LÉPÉS: Ez a változó tárolja, mit kell kihagyni (pl. Player layer)
    public LayerMask excludeLayers;

    public Slider powerSlider;
    public float sliderSpeed = 2.5f;

    public static bool canMove = true;

    private bool isSettingPower = false;
    private Ore currentlyLookedAtOre;
    private bool hitBonusPoint = false;

    void Start()
    {
        if (powerSlider != null) powerSlider.gameObject.SetActive(false);
        canMove = true;
    }

    void Update()
    {
        if (isSettingPower)
        {
            HandlePowerSlider();
        }
        else
        {
            PerformOreDetection();

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

        // Pontosan úgy, ahogy az Inventory.Interaction.cs-ben:
        // Sugár létrehozása a kamera pozíciójából elõre
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Itt a titok: a ~ (tilde) karakter az excludeLayers elõtt
        // Ez mondja meg a Unity-nek, hogy mindent érzékeljen, KIVÉVE a listában lévõket.
        if (Physics.Raycast(ray, out hit, miningRange, ~excludeLayers))
        {
            // Az Ore script keresése a szülõben is (ha több részbõl áll az érc)
            Ore ore = hit.collider.GetComponentInParent<Ore>();

            if (ore != null)
            {
                currentlyLookedAtOre = ore;

                // HitMarker (gyenge pont) ellenõrzése a collider tagje alapján
                if (hit.collider.CompareTag("HitMarker"))
                {
                    hitBonusPoint = true;
                }
            }
        }
    }

    // ... (A többi metódus: StartMiningPowerSequence, HandlePowerSlider, FinalizeMiningHit változatlan marad)

    private void StartMiningPowerSequence()
    {
        Vector3 lookTarget = currentlyLookedAtOre.transform.position;
        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);

        isSettingPower = true;
        canMove = false;
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
        float damageMultiplier = 1f;

        if (power >= 0.45f && power <= 0.55f) damageMultiplier = 2.5f;
        else if ((power >= 0.2f && power < 0.45f) || (power > 0.55f && power <= 0.8f)) damageMultiplier = 1.0f;
        else damageMultiplier = 0.5f;

        if (currentlyLookedAtOre != null)
        {
            currentlyLookedAtOre.GetHit(damageMultiplier, hitBonusPoint);
        }

        isSettingPower = false;
        canMove = true;
        powerSlider.gameObject.SetActive(false);
    }
}