using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleCalendarFlip : MonoBehaviour
{
    [Header("Megjelenítés")]
    public Image displayImage;       // A Canvas-on lévő Image objektum
    public Sprite[] calendarPages;  // A hónapok képei

    [Header("Hangok")]
    public AudioSource audioSource; // Az AudioSource komponens
    public AudioClip flipSound;    // A papír suhogás hangfájl

    private int currentIndex = 0;

    public void FlipPage()
    {
        if (calendarPages.Length == 0) return;

        currentIndex = (currentIndex + 1) % calendarPages.Length;

        if (displayImage != null)
        {
            displayImage.sprite = calendarPages[currentIndex];
        }

        if (audioSource != null && flipSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); 
            audioSource.PlayOneShot(flipSound);
        }

        StopAllCoroutines(); // Megállítjuk az előzőt, ha túl gyorsan nyomkodod
        StartCoroutine(PunchAnimation());

        Debug.Log("Lapozva ide: " + calendarPages[currentIndex].name);
    }

    IEnumerator PunchAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 punchScale = originalScale * 1.1f; // 10%-kal nagyobb lesz

        // Növekedés
        transform.localScale = punchScale;

        // Visszaállás fokozatosan
        float elapsed = 0f;
        float duration = 0.2f; // 0.2 másodperc alatt megy vissza
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(punchScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}