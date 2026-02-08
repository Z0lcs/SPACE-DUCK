using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmelterUI : MonoBehaviour
{
    [Header("References")]
    public Smelter targetSmelter;

    [Header("UI Elements")]
    public Slider tempSlider;
    public Slider pressureSlider;
    public Image progressOverlay;
    public TextMeshProUGUI statusText;

    [Header("Buttons")]
    public Button fuelButton;
    public Button waterButton;

    private void Start()
    {
        fuelButton.onClick.AddListener(OnFuelClick);
        waterButton.onClick.AddListener(OnWaterClick);

        tempSlider.minValue = 0;
        tempSlider.maxValue = 1200;
        pressureSlider.minValue = 0;
        pressureSlider.maxValue = 100;
    }

    void Update()
    {
        // Ha nincs session, alaphelyzetbe állítjuk az UI-t
        if (targetSmelter == null || targetSmelter.currentSession == null)
        {
            tempSlider.value = Mathf.Lerp(tempSlider.value, 20f, Time.deltaTime);
            pressureSlider.value = Mathf.Lerp(pressureSlider.value, 0f, Time.deltaTime);
            if (progressOverlay != null) progressOverlay.fillAmount = 0;
            statusText.text = "VÁRAKOZÁS...";
            return;
        }

        var session = targetSmelter.currentSession;
        tempSlider.value = session.temp;
        pressureSlider.value = session.pressure;

        // Haladás kijelzése
    //    if (targetSmelter.inputSlot.HasItem())
    //    {
    //        float totalTarget = targetSmelter.inputSlot.GetItem().smeltingTime + session.penaltyTime;
    //        if (progressOverlay != null) progressOverlay.fillAmount = session.progress / totalTarget;
    //    }

    //    UpdateStatusText(session);
    }

    private void UpdateStatusText(SmelterSession s)
    {
        if (s.temp > 800) statusText.text = "<color=red>KRITIKUS HÕ!</color>";
        else if (s.temp < 400) statusText.text = "<color=blue>ALACSONY HÕ</color>";
        else statusText.text = "<color=green>OPTIMÁLIS OLVASZTÁS</color>";
    }

    private void OnFuelClick()
    {
        if (targetSmelter.currentSession != null && targetSmelter.fuelSlot.HasItem())
        {
            targetSmelter.currentSession.AddHeat(targetSmelter.fuelSlot.GetItem().fuelValue);
            targetSmelter.fuelSlot.RemoveAmount(1);
        }
    }

    private void OnWaterClick()
    {
        if (targetSmelter.currentSession != null)
        {
            targetSmelter.currentSession.CoolDown();
        }
    }
}