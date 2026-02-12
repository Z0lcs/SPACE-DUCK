using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip music1; // Ide húzd az első zenét
    public AudioClip music2; // Ide húzd a másodikat
    public float breakTime = 5f; // Szünet a zenék között (Minecraft-élmény)

    private bool playFirst = true;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayMusicRoutine());
    }

    IEnumerator PlayMusicRoutine()
    {
        while (true)
        {
            // Választunk egy számot
            AudioClip clipToPlay = playFirst ? music1 : music2;
            audioSource.clip = clipToPlay;

            // Lejátszás
            audioSource.Play();
            Debug.Log("Most szól: " + clipToPlay.name);

            // Megvárjuk, amíg véget ér a zene
            yield return new WaitForSeconds(clipToPlay.length);

            // Szünet tartása a következő előtt
            yield return new WaitForSeconds(breakTime);

            // Váltunk a másikra
            playFirst = !playFirst;
        }
    }
}