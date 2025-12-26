using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement controls")]
    public float speed = 5f;
    private Vector2 movement;
    private bool canMove = true;

    private Rigidbody2D rb;

    [Header("Jump controls")]
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool jumpRequest;

    [Header("Ground controls")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;

    public Transform lightPos;

    [Header("Take damage controls")]
    private bool isKnockback;
    public float knockbackDuration = 0.2f;
    public float knockbackForce = 45f;

    public int health = 3;
    public int enemyLayer;
    private bool instantKill = false;

    public Vector2 boxSize = new Vector2(0.5f, 0.1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequest = true;

        GroundControl();
        JumpAnim();
        TakeDamage();
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
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
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
        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Vector3 center = groundCheck.position + (Vector3.down * groundCheckDistance * 0.5f);
        Vector3 size = new Vector3(boxSize.x, groundCheckDistance, 1f);
        Gizmos.DrawWireCube(center, size);
    }

    IEnumerator ApplyKnockback(Vector2 knockbackDir)
    {
        isKnockback = true;
        canMove = false;

        rb.velocity = Vector2.zero;

        Vector2 force = new Vector2(knockbackDir.x * knockbackForce, 0);

        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);

        canMove = true;
        isKnockback = false;
    }

    public void TakeDamage()
    {
        if (health == 0)
        {
            Destroy(gameObject);
        }

        if (instantKill == true)
        {
            health = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDamage(collision.gameObject);

        if (collision.collider.CompareTag("InstantKill"))
        {
            instantKill = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WeakPoint"))
        {
            return;
        }

        HandleDamage(collision.gameObject);

        if (collision.CompareTag("Destroyable"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void HandleDamage(GameObject attacker)
    {
        if (attacker.layer == enemyLayer && !isKnockback)
        {
            health--;
            Debug.Log("Current health: " + health);

            float dirX = transform.position.x - attacker.transform.position.x;
            Vector2 knockbackDir = new Vector2(Mathf.Sign(dirX), 0);

            StartCoroutine(ApplyKnockback(knockbackDir));
        }
    }
}
