using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    public Slider masterSlider, bgSlider, questSlider, expSlider;


    void Start()
    {
        masterSlider.value = 70f;
        bgSlider.value = 70f;
        questSlider.value = 70f;
        expSlider.value = 70f;

        ApplyVolume("Master", 70f);
        ApplyVolume("BG", 70f);
        ApplyVolume("Quest", 70f);
        ApplyVolume("Explosion", 70f);
    }

    public void SetMasterVolume(float val) => ApplyVolume("Master", val);
    public void SetBGVolume(float val) => ApplyVolume("BG", val);
    public void SetExplosionVolume(float val) => ApplyVolume("Explosion", val);
    public void SetQuestVolume(float val) => ApplyVolume("Quest", val);

    private void ApplyVolume(string parameterName, float value)
    {

        if (value <= 0.01f)
        {
            mainMixer.SetFloat(parameterName, -80f);
        }
        else
        {
            float normalizedValue = value / 100f;
            float dB = Mathf.Log10(normalizedValue) * 20;

            if (parameterName == "Master")
            {
                mainMixer.SetFloat(parameterName, dB);
            }
            else
            {
                mainMixer.SetFloat(parameterName, dB - 10f);
            }
        }
    }
}