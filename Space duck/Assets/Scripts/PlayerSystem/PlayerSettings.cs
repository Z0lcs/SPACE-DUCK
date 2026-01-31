using UnityEngine;
using Unity.Cinemachine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings Instance;

    public static float currentSpeedMultiplier = 1f;
    public static float sensitivityMultiplier = 1f;
    public static float fovValue = 60f;

    [SerializeField] private CinemachineCamera freeLookCamera;

    void Awake()
    {
        Instance = this;

        sensitivityMultiplier = PlayerPrefs.GetFloat("Sensitivity", 1f);
        currentSpeedMultiplier = PlayerPrefs.GetFloat("Speed", 1f);
        fovValue = PlayerPrefs.GetFloat("FOV", 60f);

        ApplyFOV();
    }
    public void ApplyFOV()
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.Lens.FieldOfView = fovValue;
        }
    }
}