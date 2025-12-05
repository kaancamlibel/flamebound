using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Rigidbody2D rb;
    public bool goLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (goLeft)
            rb.velocity = Vector2.left * speed;
        else
            rb.velocity = Vector2.right * speed;

        Destroy(gameObject, 5f); // Destroy bullet after 5 seconds
    }
}
