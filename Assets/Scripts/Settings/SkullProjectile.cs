using System.Collections;
using UnityEngine;

public class SkullProjectile : MonoBehaviour
{
    [Header("///")]
    public GameObject skullPrefab;
    public float waitTime = 1.0f;
    public float spawnTime = 1.0f;
    public Transform spawnPoint;
    public float totalDuration = 5.0f;

    private Coroutine spawnCoroutine;
    public GameObject cursedVFX;

    private void OnEnable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

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

        gameObject.SetActive(false);
        cursedVFX.SetActive(false);
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}