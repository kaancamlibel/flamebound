using UnityEngine;
using System.Collections;

public class BombControl : MonoBehaviour
{
    public GameObject bombPrefab; 
    public float respawnDelay = 3f;
    public GameObject targetObject;

    private void Start()
    {
        SpawnBomb();
    }

    void SpawnBomb()
    {
        GameObject newBomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);

        ForceBomb fb = newBomb.GetComponent<ForceBomb>();
        if (fb != null)
        {
            fb.OnBombDestroyed += () => StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (targetObject != null)
        {
            SpawnBomb();
        }
        else
        {
            Debug.Log("Hedef obje yok olduðu için yeni bomba doðmayacak.");
        }
    }
}