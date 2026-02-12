using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public RectTransform handle;
    public Image handleImage;

    [Header("Méretezés")]
    public float minScale = 1.0f;
    public float maxScale = 2.5f;

    [Header("Képváltás (Opcionális)")]
    public Sprite evolvedSprite;
    private Sprite originalSprite;
    [Range(0, 1)] public float changeThreshold = 0.7f;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (handleImage == null && handle != null) handleImage = handle.GetComponent<Image>();

        if (handleImage != null) originalSprite = handleImage.sprite;

        slider.onValueChanged.AddListener(OnSliderChanged);

        OnSliderChanged(slider.value);
    }

    void OnSliderChanged(float value)
    {
        if (handle == null) return;

        float normalizedValue = (value - slider.minValue) / (slider.maxValue - slider.minValue);

        handle.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, normalizedValue);

        if (evolvedSprite != null && handleImage != null)
        {
            if (normalizedValue >= changeThreshold)
            {
                handleImage.sprite = evolvedSprite;
            }
            else
            {
                handleImage.sprite = originalSprite;
            }
        }
    }
}