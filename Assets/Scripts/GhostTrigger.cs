using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    public GameObject ghost;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!ghost) return;
            ghost.SetActive(true);
        }
    }
}
