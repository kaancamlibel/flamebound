using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float baseSpeed;     
    public float chaseMultiplier = 2f;

    public float detectionRange = 0.5f;
    public float wallDetectionRange = 0.5f;
    public float playerDetectionRange = 5f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public Transform groundCheck;
    public Transform wallCheck;

    private bool facingRight = true;

    private Animator animator;

    public float patrolStopInterval = 5f;   // 5 saniyede bir duracak
    public float stopDuration = 1.5f;       // Kaç saniye duracaðýný ayarla
    private bool isPaused = false;
    private bool playerInSight = false;
    private float patrolTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        movement.x = 1f;
        facingRight = true;

        baseSpeed = moveSpeed;
    }

    void Update()
    {
        Patrol();
        CheckGround();
        CheckWall();
        CheckPlayer();
    }

    private void Patrol()
    {
        if (!playerInSight)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolStopInterval && !isPaused)
            {
                StartCoroutine(PausePatrol());
            }
        }

        // Düþman durma modunda ise hareketi tamamen kes
        if (isPaused)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetFloat("speed", 0f);
            return;
        }

        // Normal patrol veya chase hareketi
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(movement.x));
    }

    private void CheckGround()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            detectionRange,
            groundLayer
        );

        Debug.DrawRay(groundCheck.position, Vector2.down * detectionRange, Color.red);

        if (!groundInfo)
        {
            Flip();
        }
    }

    private void CheckWall()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right,
            wallDetectionRange,
            groundLayer
        );

        Debug.DrawRay(wallCheck.position, Vector2.right * wallDetectionRange, Color.red);

        if (groundInfo)
        {
            Flip();
        }
    }

    private void CheckPlayer()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            playerDetectionRange,
            playerLayer
        );

        Debug.DrawRay(transform.position, direction * playerDetectionRange, Color.blue);

        if (hit.collider != null)
        {
            playerInSight = true;
            moveSpeed = baseSpeed * chaseMultiplier;

            patrolTimer = 0f;
            isPaused = false;
        }
        else 
        {
            playerInSight = false;
            moveSpeed = baseSpeed;
        }
    }

    IEnumerator PausePatrol()
    {
        isPaused = true;

        yield return new WaitForSeconds(stopDuration);

        // Durma bitti
        isPaused = false;

        patrolTimer = 0f;
    }

    private void Flip()
    {
        facingRight = !facingRight;

        // Yönü tersine çevir
        movement.x = facingRight ? 1f : -1f;

        // Sprite'ý çevir
        transform.Rotate(0f, 180f, 0f);
    }
}