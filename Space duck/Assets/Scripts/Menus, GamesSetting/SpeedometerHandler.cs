using UnityEngine;
using UnityEngine.UI;

public class SpeedometerHandler : MonoBehaviour
{
    public Slider speedSlider;     
    public RectTransform needle;   

    public float minAngle = 0f;     
    public float maxAngle = -180f; 

    void Update()
    {
        float speedValue = speedSlider.value / speedSlider.maxValue;

        float currentAngle = Mathf.Lerp(minAngle, maxAngle, speedValue);

        needle.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }
}