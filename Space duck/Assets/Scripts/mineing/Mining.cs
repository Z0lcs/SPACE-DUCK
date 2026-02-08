using UnityEngine;
using UnityEngine.UI;

public class Mining : MonoBehaviour
{
    [Header("Beállítások")]
    public float interactionDistance = 3f;
    public Transform playerCamera;
    public Slider powerSlider;
    public float sliderSpeed = 2.5f;

    [Header("Animáció")]
    public Animator anim;

    private bool isSettingPower = false;
    private Ore currentOre;
    private bool hitBonusPoint = false;

    void Start()
    {
        powerSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isSettingPower)
        {
            // Csúszka mozgatása
            powerSlider.value = Mathf.PingPong(Time.time * sliderSpeed, 1f);

            if (Input.GetMouseButtonDown(0))
            {
                FinalizeHit();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckForOre();
            }
        }
    }

    void CheckForOre()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance + 5f))
        {
            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist <= interactionDistance)
            {
                currentOre = hit.transform.GetComponentInParent<Ore>();
                if (currentOre != null)
                {
                    hitBonusPoint = hit.transform.CompareTag("HitMarker");

                    // Fordulás az érchez
                    Vector3 lookTarget = hit.point;
                    lookTarget.y = transform.position.y;
                    transform.LookAt(lookTarget);

                    // Animáció indítása
                    if (anim != null) anim.SetTrigger("Mine");

                    isSettingPower = true;
                    powerSlider.gameObject.SetActive(true);
                }
            }
        }
    }

    void FinalizeHit()
    {
        float power = powerSlider.value;
        // Ha a csúszka végén áll meg (Sweet spot)
        if (power > 0.9f) power *= 2f;

        currentOre.GetHit(power, hitBonusPoint);

        isSettingPower = false;
        powerSlider.gameObject.SetActive(false);
        currentOre = null;
    }
}