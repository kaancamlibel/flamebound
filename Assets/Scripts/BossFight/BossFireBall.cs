using UnityEngine;

public class BossFireBall : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Vector2 direction = (playerObj.transform.position - transform.position).normalized;

            rb.velocity = direction * speed;
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Oyuncu vuruldu!");
            Destroy(gameObject); 
        }
    }
}