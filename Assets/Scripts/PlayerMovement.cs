using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Speed of the player movement
    public float speed = 5f;
    private Vector2 movement;

    private Rigidbody2D rb;

    // Jump force
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool jumpRequest;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        movement.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequest = true;

        JumpAnim(); 
    }

    private void FixedUpdate()
    {
        MoveCharacter();

        if (jumpRequest)
        {
            ApplyJump();
            jumpRequest = false;
        }
    }

    void MoveCharacter()
    {
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

        if (movement.x > 0)
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;

        animator.SetFloat("isRunning", Mathf.Abs(movement.x));
    }

    void ApplyJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void JumpAnim()
    {
        if (!isGrounded)
        {
            if (rb.velocity.y > 0.1f)
            {
                animator.SetBool("jumpUp", true);
                animator.SetBool("jumpDown", false);
            }
            else if (rb.velocity.y < -0.1f)
            {
                animator.SetBool("jumpDown", true);
                animator.SetBool("jumpUp", false);
            }
        }
        else
        {
            animator.SetBool("jumpUp", false);
            animator.SetBool("jumpDown", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
