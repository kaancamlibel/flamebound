using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Speed of the player movement
    public float speed = 5f;
    private Vector2 movement;
    private bool canMove = true;

    private Rigidbody2D rb;

    // Jump force
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool jumpRequest;

    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public Transform lightPos;
    private Vector2 lightDefaultPos;

    private bool isKnockback;
    public float knockbackDuration = 0.2f;
    public float knockbackForce = 45f;
    public float knockbackUpForce = 0.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        lightDefaultPos = lightPos.localPosition;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequest = true;

        GroundControl();
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
        if (!canMove) return;

        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
            lightPos.localPosition = lightDefaultPos;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
            lightPos.localPosition = new Vector2(-lightDefaultPos.x, lightDefaultPos.y);
        }

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

    private void GroundControl()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.blue);

        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    IEnumerator ApplyKnockback(Vector2 knockbackDir)
    {
        isKnockback = true;
        canMove = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);

        canMove = true;
        isKnockback = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isKnockback)
        {
            Debug.Log("Player collided with Enemy, applying knockback.");

            Vector2 knockbackDir =
                (transform.position - collision.transform.position).normalized;

            knockbackDir.y = knockbackUpForce;
            knockbackDir.Normalize();

            StartCoroutine(ApplyKnockback(knockbackDir));
        }
    }
}
