using UnityEngine;
using System.Collections;

public class KidnapTrigger : MonoBehaviour
{
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
    public float waveInterval = 1f;      // Her dalga arasý bekleme
    public float delayBetweenSpawns = 0.5f; // Düþmanlarýn tek tek doðmasý için ara bekleme
    public float eventDuration = 30f;    // Savaþýn toplam kaç saniye süreceði

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

        Debug.Log("Iþýk ulaþtý! Savaþ baþlýyor...");

        slimePrefab.SetActive(true);

        // Savaþ dalgalarýný baþlat
        StartCoroutine(SpawnWaves());

        // Etkinliði bitirmek için geri sayýmý baþlat
        StartCoroutine(EndEventAfterTime());
    }

    IEnumerator SpawnWaves()
    {
        // hasTriggered true olduðu sürece (veya süre bitene kadar) devam eder
        while (hasTriggered)
        {
            // Düþmanlarý belli aralýklarla sýrayla doður (Daha doðal görünür)
            Instantiate(lizardPrefab, lizardPointB.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenSpawns);

            Instantiate(skeletonPrefab, skeletonPointA.position, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenSpawns);

            // Dalga bitti, bir sonraki dalga için bekle
            yield return new WaitForSeconds(waveInterval);
        }
    }

    IEnumerator EndEventAfterTime()
    {
        // Toplam süre kadar bekle
        yield return new WaitForSeconds(eventDuration);

        // Savaþý durdur
        hasTriggered = false;

        voidEffect.SetActive(false);
        voidWall.SetActive(false);
        slimePrefab.SetActive(false);

        StopCoroutine(SpawnWaves());

        Debug.Log("Savaþ bitti! Iþýk serbest kalýyor.");

        lightCtrl.isKidnapped = false;
    }
}