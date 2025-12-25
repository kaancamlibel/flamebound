using UnityEngine;

public class EnemyFlyAI : MonoBehaviour
{
    public float speed = 4f;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            // Flip sprite based on movement direction
            if (direction.x > 0)
                spriteRenderer.flipX = true;
            else if (direction.x < 0)
                spriteRenderer.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // CursedManager'a ulaþýp laneti baþlatmasýný söylüyoruz
            if (CursedManager.Instance != null)
            {
                CursedManager.Instance.StartCurse();
            }

            // Hayaleti yok et
            Destroy(gameObject, 0.3f);
        }
    }
}
