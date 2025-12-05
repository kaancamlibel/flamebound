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
    private bool isJumping;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Attack
    public GameObject bulletPrefab;
    public Transform bulletPosRight;
    public Transform bulletPosLeft;
    public float attackWait = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Initialize variables if needed
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequest = true;

        if (Input.GetButtonDown("Fire1") && !isJumping)
        {
            bool isStandingStill = Mathf.Abs(movement.x) < 0.01f;

            if (isStandingStill)
            {
                canMove = false;
                StartCoroutine(WaitTime());
            }

            animator.SetTrigger(movement.x == 0f ? "attack" : "runAttack");

            FireBullet();
        }

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

    void FireBullet()
    {
        Transform spawnPos = spriteRenderer.flipX ? bulletPosLeft : bulletPosRight;

        var bullet = Instantiate(bulletPrefab, spawnPos.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().goLeft = spriteRenderer.flipX;
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(attackWait);
        canMove = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isJumping = true;
        }
    }
}
