using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private AudioSource audioSource;
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private float fadeDuration;

    public bool isPlayingBossMusic {get; private set;}
    private float normalVolume;
    private float normalFadeDuration;

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

    public void FadeToBossMusic()
    {
        StopCoroutine("FadeMusic");
        normalVolume = audioSource.volume;
        normalFadeDuration = fadeDuration;
        isPlayingBossMusic = true;
        fadeDuration = 12.8f;
        StartCoroutine(FadeMusic(bossMusic, endingVolume: 1f));
        audioSource.volume = normalVolume;
    }

    public void StopBossMusic() {
        isPlayingBossMusic = false;
        audioSource.volume = normalVolume;
        fadeDuration = normalFadeDuration;
        WaveManager.Instance.DecideMusic();
    }

    private IEnumerator FadeMusic(AudioClip newClip, float endingVolume = -1f)
    {
        Debug.Log("Fading Start");
        // Fade out current music
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // // Wait for a couple of seconds
        // yield return new WaitForSeconds(1f);
        audioSource.volume = 0;
        audioSource.Stop();

        Debug.Log("Fading Middle");

        // Switch to new music and fade in
        audioSource.clip = newClip;
        audioSource.volume = endingVolume;
        audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, endingVolume < 0 ? startVolume : endingVolume, t / fadeDuration);

            yield return null;
        }
        Debug.Log("Fading EndDebug.Log");
        audioSource.volume = endingVolume < 0 ? startVolume : endingVolume;
    }
}