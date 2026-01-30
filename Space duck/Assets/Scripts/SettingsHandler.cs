using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine; 

public class SettingsHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;

    [Header("Sliders")]
    public Slider speedSlider;
    public Slider sensitivitySlider;
    public Slider fovSlider;

    [Header("References")]
    public CinemachineCamera vCam;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);

        speedSlider.value = PlayerMove.currentSpeedMultiplier;
        sensitivitySlider.value = PlayerMove.sensitivityMultiplier;

        if (vCam != null)
            fovSlider.value = vCam.Lens.FieldOfView;

        speedSlider.onValueChanged.AddListener(UpdateSpeed);
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        fovSlider.onValueChanged.AddListener(UpdateFOV);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;

        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null) player.TogglePhysics(true);

        if (vCam != null) vCam.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null) player.TogglePhysics(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateSpeed(float val) => PlayerMove.currentSpeedMultiplier = val;
    void UpdateSensitivity(float val) => PlayerMove.sensitivityMultiplier = val;

    void UpdateFOV(float val)
    {
        if (vCam != null)
        {
            var lens = vCam.Lens;
            lens.FieldOfView = val;
            vCam.Lens = lens;
        }
    }
}