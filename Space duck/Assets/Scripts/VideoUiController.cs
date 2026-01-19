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

    void Start()
    {
    }

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

    public void PlayIntro()
    {
        if (introClip != null)
        {
            PlayVideo(introClip);
        }
    }

    public void StartQuestVideoSequence(float delay = 15f)
    {
        StartCoroutine(DelayedQuestVideo(delay));
    }

    private IEnumerator DelayedQuestVideo(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (questClip != null)
        {
            PlayVideo(questClip);
        }
    }

    private void PlayVideo(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();
    }

    void HideUI(VideoPlayer source)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
    }

    void ShowUI(VideoPlayer source)
    {
        if (mainCanvas != null) mainCanvas.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
    }
}