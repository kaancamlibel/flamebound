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

    [Header("Settings")]
    public float waveInterval = 1f;      
    public float delayBetweenSpawns = 0.5f; 
    public float eventDuration = 30f;    

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            voidEffect.SetActive(true);
            lightCtrl.StartKidnapping(finalBattlePoint.position, kidnapSpeed);
            StartCoroutine(WaitAndStartFight());
        }
    }

    IEnumerator WaitAndStartFight()
    {
        while (Vector2.Distance(lightCtrl.transform.position, finalBattlePoint.position) > 0.5f)
        {
            yield return null;
        }

        slimePrefab.SetActive(true);

        StartCoroutine(SpawnWaves());

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

        hasTriggered = false;

        voidEffect.SetActive(false);
        voidWall.SetActive(false);
        slimePrefab.SetActive(false);

        StopCoroutine(SpawnWaves());

        lightCtrl.isKidnapped = false;
    }
}