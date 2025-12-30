using System.Collections;
using UnityEngine;

public class TornadoSpawner : MonoBehaviour
{
    [Header("Spawner Points")]
    public Transform spawnerA;
    public Transform spawnerB;
    public GameObject tornadoPrefab;

    [Header("Settings")]
    public float tornadoSpeed = 5f;
    public float spawnInterval = 10f;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            SpawnAndLaunch(spawnerA);
            yield return new WaitForSeconds(spawnInterval);

            SpawnAndLaunch(spawnerB);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAndLaunch(Transform spawnPoint)
    {
        if (tornadoPrefab == null) return;

        GameObject tornado = Instantiate(tornadoPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = tornado.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.right * tornadoSpeed;
        }

        Destroy(tornado, 5f);
    }
}