using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    public Slider masterSlider, bgSlider, questSlider, expSlider;

    private bool isInitialized = false;

    void Awake()
    {
        // Először beállítjuk a csúszkák vizuális értékét
        masterSlider.value = 0.7f;
        bgSlider.value = 0.7f;
        questSlider.value = 0.7f;
        expSlider.value = 0.7f;

        // Kényszerítjük a Mixert a 70%-ra (-10dB korláttal)
        ApplyVolume("Master", 0.7f);
        ApplyVolume("BG", 0.7f);
        ApplyVolume("Quest", 0.7f);
        ApplyVolume("Explosion", 0.7f);

        isInitialized = true;
    }

    // Ezeket hívják a Sliderek (On Value Changed)
    public void SetMasterVolume(float val) => ApplyVolume("Master", val);
    public void SetBGVolume(float val) => ApplyVolume("BG", val);
    public void SetExplosionVolume(float val) => ApplyVolume("Explosion", val);
    public void SetQuestVolume(float val) => ApplyVolume("Quest", val);

    private void ApplyVolume(string parameterName, float value)
    {
        // Ha még nem inicializáltunk, ne engedjük a Slidereknek elrontani
        if (!isInitialized && Time.timeSinceLevelLoad < 0.1f) return;

        if (value <= 0.0001f)
        {
            mainMixer.SetFloat(parameterName, -80f);
        }
        else
        {
            // A te bevált -10dB-es eltolásod
            float dB = (Mathf.Log10(value) * 20) - 10f;
            mainMixer.SetFloat(parameterName, dB);
        }
    }
}