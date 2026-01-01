using UnityEngine;
using System.Collections;

public class KidnapTrigger : MonoBehaviour
{
    [Header("Game Objects")]
    public LightController lightCtrl;
    public Transform finalBattlePoint;
    public float kidnapSpeed = 8f;
    public GameObject voidEffect;
    public GameObject voidWall;

    [Header("Spawn Points")]
    public Transform skeletonPointA;
    public Transform lizardPointB;

    [Header("Prefabs")]
    public GameObject lizardPrefab;
    public GameObject skeletonPrefab;
    public GameObject slimePrefab;

    public GameObject fireHeartL;

    [Header("Settings")]
    public float waveInterval = 1f;
    public float delayBetweenSpawns = 0.5f;
    public float eventDuration = 30f;

    private bool hasTriggered = false;
    private Coroutine fightCoroutine; 
    private Coroutine wavesCoroutine;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    public AudioClip startSound;
    public AudioClip backgroundMusic;
    public MusicPlaylistLoop backgroundPlaylist;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.loop = true;
    }

    private void Start()
    {
        if (voidWall != null) voidWall.SetActive(true);

        if (voidEffect != null) voidEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            StartEvent();
        }
    }

    public void StartEvent()
    {
        if (backgroundPlaylist != null) backgroundPlaylist.PauseMusic();

        hasTriggered = true;
        voidEffect.SetActive(true);
        voidWall.SetActive(true);

        if (audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }

        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }

        lightCtrl.StartKidnapping(finalBattlePoint.position, kidnapSpeed);
        fightCoroutine = StartCoroutine(WaitAndStartFight());
    }

    public void ResetTrigger()
    {
        if (backgroundPlaylist != null) backgroundPlaylist.ResumeMusic();

        StopAllCoroutines();
        hasTriggered = false;

        if (audioSource != null) audioSource.Stop();

        voidEffect.SetActive(false);

        if (slimePrefab != null) slimePrefab.SetActive(false);
    }

    IEnumerator WaitAndStartFight()
    {
        while (Vector2.Distance(lightCtrl.transform.position, finalBattlePoint.position) > 0.5f)
        {
            yield return null;
        }

        if (slimePrefab != null) slimePrefab.SetActive(true);
        wavesCoroutine = StartCoroutine(SpawnWaves());
        StartCoroutine(EndEventAfterTime());
    }

    IEnumerator SpawnWaves()
    {
        while (hasTriggered)
        {
            Instantiate(lizardPrefab, lizardPointB.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenSpawns);

            Instantiate(skeletonPrefab, skeletonPointA.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenSpawns);

            yield return new WaitForSeconds(waveInterval);
        }
    }

    IEnumerator EndEventAfterTime()
    {
        yield return new WaitForSeconds(eventDuration);
        FinishEvent();
    }

    void FinishEvent()
    {
        if (backgroundPlaylist != null) backgroundPlaylist.ResumeMusic();

        hasTriggered = false;

        if (audioSource != null) audioSource.Stop();

        fireHeartL.SetActive(true);
        voidEffect.SetActive(false);
        voidWall.SetActive(false);

        if (slimePrefab != null) slimePrefab.SetActive(false);
        lightCtrl.isKidnapped = false;
    }
}