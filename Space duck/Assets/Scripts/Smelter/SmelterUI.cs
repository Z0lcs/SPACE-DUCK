using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmelterUI : MonoBehaviour
{
    public Smelter targetSmelter;
    public Slider tempSlider;
    public Slider pressureSlider;
    public Image progressOverlay;
    public TextMeshProUGUI statusText;

    [Header("Buttons")]
    public Button fuelButton;
    public Button waterButton;
    public Button emergencyButton;

    private void Awake()
    {
        // Tisztítjuk a régi listener-eket, hogy ne legyen duplázódás
        fuelButton.onClick.RemoveAllListeners();
        waterButton.onClick.RemoveAllListeners();

        // Újrabindoljuk õket
        fuelButton.onClick.AddListener(OnFuelClick);
        waterButton.onClick.AddListener(OnWaterClick);

        if (emergencyButton != null)
        {
            emergencyButton.onClick.RemoveAllListeners();
            emergencyButton.onClick.AddListener(() => targetSmelter.EmergencyShutdown());
        }
    }

    void Update()
    {
        if (targetSmelter == null || targetSmelter.currentSession == null)
        {
            // Ha nincs aktív olvasztás, lassan hûljön le az UI kijelzõje is
            tempSlider.value = Mathf.Lerp(tempSlider.value, 20f, Time.deltaTime);
            pressureSlider.value = Mathf.Lerp(pressureSlider.value, 0f, Time.deltaTime);
            if (progressOverlay != null) progressOverlay.fillAmount = 0;
            statusText.text = "VÁRAKOZÁS...";
            return;
        }

        var s = targetSmelter.currentSession;
        tempSlider.value = s.temp;
        pressureSlider.value = s.pressure;

        if (targetSmelter.inputSlot.HasItem())
        {
            float total = targetSmelter.inputSlot.GetItem().smeltingTime + s.penaltyTime;
            if (progressOverlay != null) progressOverlay.fillAmount = s.progress / total;
        }

        UpdateStatusText(s);
    }

    private void UpdateStatusText(SmelterSession s)
    {
        if (s.temp > 800) statusText.text = "<color=red>KRITIKUS HÕ!</color>";
        else if (s.temp < 400) statusText.text = "<color=blue>ALACSONY HÕ</color>";
        else statusText.text = "<color=green>ÉGETÉS...</color>";
    }

    public void OnFuelClick()
    {
        Debug.Log("Fûtés gomb megnyomva!"); // Konzolban látni fogod, ha mûködik
        if (targetSmelter.currentSession != null)
        {
            if (targetSmelter.fuelSlot.HasItem())
            {
                targetSmelter.currentSession.AddHeat(targetSmelter.fuelSlot.GetItem().fuelValue);
                targetSmelter.fuelSlot.RemoveAmount(1);
            }
            else
            {
                Debug.LogWarning("Nincs üzemanyag!");
            }
        }
    }

    public void OnWaterClick()
    {
        Debug.Log("Hûtés gomb megnyomva!");
        targetSmelter.currentSession?.CoolDown();
    }
}