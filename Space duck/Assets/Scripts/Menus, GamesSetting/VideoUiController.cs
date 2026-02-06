using UnityEngine;
using UnityEngine.Video;

public class VideoUiController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject mainCanvas;

    [Header("Beállítások")]
    public VideoClip introClip;
    public VideoClip questClip;

    public bool isQuestCompleted = false;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += HideUI;
            videoPlayer.loopPointReached += ShowUI;
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started -= HideUI;
            videoPlayer.loopPointReached -= ShowUI;
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
        Debug.Log("Quest kész! Most már megnyomhatod az E-t a videóhoz.");
    }

    public void PlayIntro()
    {
        if (introClip != null)
        {
            PlayVideo(introClip);
        }
    }

    private void PlayVideo(VideoClip clip)
    {
        if (videoPlayer == null) return;

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void HideUI(VideoPlayer vp)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        Debug.Log("UI elrejtve, videó indul.");
    }

    void ShowUI(VideoPlayer vp)
    {
        Debug.Log("Videó véget ért, UI visszakapcsolása.");
        if (mainCanvas != null) mainCanvas.SetActive(true);

        vp.Stop();
        videoPlayer.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}