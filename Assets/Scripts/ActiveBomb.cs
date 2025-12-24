using UnityEngine;

public class ActiveBomb : MonoBehaviour
{
    public GameObject bomb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bomb) return;

        if (collision.CompareTag("Player"))
        {
            bomb.SetActive(true);
        }
    }
}
