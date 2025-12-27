using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (destination != null)
            {
                collision.transform.position = destination.position;

                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }

                Debug.Log("Iþýnlanma tamamlandý!");
            }
            else
            {
                Debug.LogWarning("Hedef (Destination) atanmamýþ!");
            }
        }
    }
}