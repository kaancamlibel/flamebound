using System.Collections;
using UnityEngine;

public class RandomBombSpawner : MonoBehaviour
{
    [Header("Bomba Ayarlarý")]
    public GameObject bombPrefab;
    public float spawnDelay = 2f;

    [Header("Spawn Noktalarý")]
    public Transform[] spawnPoints;

    private GameObject currentBomb;
    private bool isWaiting = false;

    private void OnEnable()
    {
        isWaiting = false;
        currentBomb = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines(); 
    }

    void Update()
    {
        if (currentBomb == null && !isWaiting)
        {
            StartCoroutine(SpawnNewBombRoutine());
        }
    }

    IEnumerator SpawnNewBombRoutine()
    {
        isWaiting = true;

        yield return new WaitForSeconds(spawnDelay);

        if (!gameObject.activeInHierarchy) yield break;

        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform selectedPoint = spawnPoints[randomIndex];

            currentBomb = Instantiate(bombPrefab, selectedPoint.position, Quaternion.identity);

            Debug.Log("Yeni bomba doðdu: " + selectedPoint.name);
        }
        else
        {
            Debug.LogWarning("Spawn Points listesi boþ!");
        }

        isWaiting = false;
    }

    public void ClearCurrentBomb()
    {
        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }
        isWaiting = false;
    }
}