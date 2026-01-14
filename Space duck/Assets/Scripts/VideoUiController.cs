using UnityEngine;
using UnityEngine.Video;

public class VideoUiController : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public GameObject mainCanvas;  
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

    void HideUI(VideoPlayer source)
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(false); 
    }

    void ShowUI(VideoPlayer source)
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
    }
}