using System.Collections;
using UnityEngine;

public class TornadoSpawner : MonoBehaviour
{
    [Header("///")]
    public Transform spawnerA;
    public Transform spawnerB;
    public GameObject tornadoPrefab;

    [Header("///")]
    public float tornadoSpeed = 5f; 
    public float spawnInterval = 10f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnAndLaunch(spawnerA);
            yield return new WaitForSeconds(spawnInterval);

            SpawnAndLaunch(spawnerB);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAndLaunch(Transform spawnPoint)
    {
        GameObject tornado = Instantiate(tornadoPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = tornado.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.right * tornadoSpeed;
        }

        Destroy(tornado, 25f);
    }
}