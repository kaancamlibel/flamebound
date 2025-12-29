using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlaylistLoop : MonoBehaviour
{
    [Header("Müzik Listesi")]
    public List<AudioClip> playlist;

    private AudioSource audioSource;
    private int currentIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playlist.Count > 0)
        {
            StartCoroutine(PlayMusicRoutine());
        }
    }

    IEnumerator PlayMusicRoutine()
    {
        while (true)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            audioSource.clip = playlist[currentIndex];
            audioSource.Play();

            while (audioSource.isPlaying || isPaused)
            {
                yield return null;
            }

            if (!isPaused)
            {
                currentIndex = (currentIndex + 1) % playlist.Count;
            }
        }
    }

    public void PauseMusic()
    {
        isPaused = true;
        audioSource.Pause(); 
    }

    public void ResumeMusic()
    {
        isPaused = false;
        audioSource.UnPause(); 
    }
}