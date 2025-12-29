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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        hasTriggered = true;
        voidEffect.SetActive(true);
        voidWall.SetActive(true);

        if (audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }

        lightCtrl.StartKidnapping(finalBattlePoint.position, kidnapSpeed);
        fightCoroutine = StartCoroutine(WaitAndStartFight());
    }

    public void ResetTrigger()
    {
        StopAllCoroutines();
        hasTriggered = false;

        voidEffect.SetActive(false);
        voidWall.SetActive(false);
        if (slimePrefab != null) slimePrefab.SetActive(false);

        Debug.Log("Kidnap Etkinliði Sýfýrlandý!");
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
        hasTriggered = false;
        fireHeartL.SetActive(true);
        voidEffect.SetActive(false);
        voidWall.SetActive(false);
        if (slimePrefab != null) slimePrefab.SetActive(false);
        lightCtrl.isKidnapped = false;
    }
}