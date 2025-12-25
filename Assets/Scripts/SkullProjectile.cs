using System.Collections;
using UnityEngine;

public class SkullProjectile : MonoBehaviour
{
    public GameObject skullPrefab;
    public float waitTime = 1.0f;
    public float spawnTime = 1.0f;
    public Transform spawnPoint;
    public float totalDuration = 5.0f;

    private Coroutine spawnCoroutine; // Coroutine'i takip etmek için
    public GameObject cursedVFX;

    // OnEnable, obje her SetActive(true) olduðunda otomatik çalýþýr
    private void OnEnable()
    {
        // Eðer içeride çalýþan eski bir coroutine kalmýþsa (önlem olarak) durdur
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // Yeni döngüyü baþlat
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        float timer = 0f;

        while (timer < totalDuration)
        {
            if (skullPrefab != null && spawnPoint != null)
            {
                yield return new WaitForSeconds(waitTime);

                Instantiate(skullPrefab, spawnPoint.position, spawnPoint.rotation);

                timer += spawnTime;
            }
            else
            {
                break;
            }
        }

        Debug.Log("Lanet süresi bitti, obje kapatýlýyor.");

        // Atýþ bittiðinde objeyi kapat
        gameObject.SetActive(false);
        cursedVFX.SetActive(false);
    }

    // Obje SetActive(false) olduðunda coroutine referansýný temizle
    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}