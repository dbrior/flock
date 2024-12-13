using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private AudioSource audioSource;
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private float fadeDuration;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void GameOver()
    {
        audioSource.Stop();
        audioSource.volume = 1f;
        audioSource.PlayOneShot(gameOverMusic);
    }

    public void FadeToDayMusic()
    {
        StartCoroutine(FadeMusic(dayMusic));
    }

    public void FadeToNightMusic()
    {
        StartCoroutine(FadeMusic(nightMusic));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out current music
        float startVolume = audioSource.volume;
        float currVolume = startVolume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        // audioSource.volume = 0;
        // audioSource.Stop();

        // Wait for a couple of seconds
        yield return new WaitForSeconds(2);
        audioSource.volume = 0;
        audioSource.Stop();


        // Switch to new music and fade in
        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);

            yield return null;
        }
        audioSource.volume = startVolume;
    }

}