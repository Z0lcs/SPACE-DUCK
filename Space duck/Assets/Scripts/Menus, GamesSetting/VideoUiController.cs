using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoUiController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject mainCanvas;

    [Header("Beállítások")]
    public VideoClip introClip;
    public VideoClip questClip;

    public bool isQuestCompleted = false;

    void OnEnable()
    {
        videoPlayer.started += HideUI;
        videoPlayer.loopPointReached += ShowUI;
    }

    void OnDisable()
    {
        videoPlayer.started -= HideUI;
        videoPlayer.loopPointReached -= ShowUI;
    }

    public void PlayQuestVideo()
    {
        if (isQuestCompleted && questClip != null)
        {
            PlayVideo(questClip);
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
        videoPlayer.clip = clip;
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();
    }

    void HideUI(VideoPlayer vp)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
    }

    void ShowUI(VideoPlayer vp)
    {
        if (mainCanvas != null) mainCanvas.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
    }
}