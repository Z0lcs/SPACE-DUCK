using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsVariable : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;

    [Header("Beállítások")]
    public float minValue = 0.1f;
    public float maxValue = 5.0f;
    public string saveKey = "SettingName";

    [Tooltip("Ha be van pipálva, a maximumról indul. Ha nincs, akkor középről.")]
    public bool isVolume = false;

    void Start()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;

        float defaultValue = isVolume ? maxValue : (minValue + maxValue) / 2f;

        float savedValue = PlayerPrefs.GetFloat(saveKey, defaultValue);

        slider.value = savedValue;
        inputField.text = savedValue.ToString("0.##");

        slider.onValueChanged.AddListener(UpdateFromSlider);
        inputField.onEndEdit.AddListener(UpdateFromInput);
    }

    void UpdateFromSlider(float value)
    {
        inputField.text = value.ToString("0.##");
        PlayerPrefs.SetFloat(saveKey, value);

        if (saveKey == "Speed") PlayerSettings.currentSpeedMultiplier = value;
        if (saveKey == "FOV")
        {
            PlayerSettings.fovValue = value;
            if (PlayerSettings.Instance != null)
            {
                PlayerSettings.Instance.ApplyFOV();
            }
        }
    }

    void UpdateFromInput(string text)
    {
        if (float.TryParse(text, out float result))
        {
            result = Mathf.Clamp(result, minValue, maxValue);
            slider.value = result;
            inputField.text = result.ToString("0.##");
            PlayerPrefs.SetFloat(saveKey, result);
        }
        else
        {
            inputField.text = slider.value.ToString("0.##");
        }
    }
}