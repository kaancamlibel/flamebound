using UnityEngine;

public class EnemyFlyAI : MonoBehaviour
{
    public float speed = 4f;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    public AudioClip curseSound;

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
            if (curseSound != null)
            {
                AudioSource.PlayClipAtPoint(curseSound, transform.position);
            }

            if (CursedManager.Instance != null)
            {
                CursedManager.Instance.StartCurse();
            }

            Destroy(gameObject, 0.3f);
        }
    }
}
