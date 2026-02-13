using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VideoUiController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject mainCanvas;
    public AudioMixer masterMixer;

    [Header("Beállítások")]
    public VideoClip introClip;
    public VideoClip questClip;
    public string nextSceneName; 

    [Header("Exposed Parameter Neve")]
    public string mixerParameterName = "Master"; 

    public bool isQuestCompleted = false;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += HideUI;
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started -= HideUI;
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (vp.clip == questClip)
        {
            Debug.Log("Quest videó kész, takarítás és váltás...");
            PrepareForSceneChange();
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowUI(vp);
        }
    }

    private void PrepareForSceneChange()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (masterMixer != null)
        {
            masterMixer.SetFloat(mixerParameterName, 0f);
        }
    }

    public void PlayQuestVideo()
    {
        if (isQuestCompleted && questClip != null)
        {
            PlayVideo(questClip);
        }
        else
        {
            Debug.LogWarning("A küldetés nincs kész, vagy hiányzik a videó!");
        }
    }

    public void StartQuestVideoSequence(float delay = 15f)
    {
        isQuestCompleted = true;
        Debug.Log("Quest kész! Mehet a videó.");
    }

    public void PlayIntro()
    {
        if (introClip != null) PlayVideo(introClip);
    }

    private void PlayVideo(VideoClip clip)
    {
        if (videoPlayer == null) return;

        if (clip == questClip)
        {
            MuteGameAudio(true);
        }

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void HideUI(VideoPlayer vp)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
    }

    void ShowUI(VideoPlayer vp)
    {
        if (mainCanvas != null) mainCanvas.SetActive(true);

        MuteGameAudio(false);

        vp.Stop();
        videoPlayer.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void MuteGameAudio(bool mute)
    {
        if (masterMixer != null)
        {
            float volume = mute ? -80f : 0f;
            masterMixer.SetFloat(mixerParameterName, volume);
        }
    }
}