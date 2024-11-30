using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set;}
    private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverMusic;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();    
    }

    public void GameOver() {
        audioSource.Stop();
        audioSource.volume = 1f;
        audioSource.PlayOneShot(gameOverMusic);
    }
}
