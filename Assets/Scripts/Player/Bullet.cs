using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Rigidbody2D rb;
    public bool goLeft;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (goLeft)
        {
            rb.velocity = Vector2.left * speed;
            spriteRenderer.flipX = true;
        }
        else
        {
            rb.velocity = Vector2.right * speed;
            spriteRenderer.flipX = false;
        }

        Destroy(gameObject, 5f);
    }
}
