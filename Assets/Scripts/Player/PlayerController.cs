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
    public bool isJumping;

    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Attack
    public GameObject bulletPrefab;
    public Transform bulletPosRight;
    public Transform bulletPosLeft;
    public float attackWait = 0.5f;

    public Transform lightPos;
    private Vector2 lightDefaultPos;

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
            isJumping = false;
        }
        else
        {
            isGrounded = false;
            isJumping = true;
        }
    }
}
