using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimpleAI : MonoBehaviour
{
    [Header("Speed controls")]
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float baseSpeed;     
    public float chaseMultiplier = 2f;

    [Header("Detection and Layers")]
    public float detectionRange = 0.5f;
    public float wallDetectionRange = 0.5f;
    public float playerDetectionRange = 5f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public Transform groundCheck;
    public Transform wallCheck;

    private bool facingRight = true;

    private Animator animator;

    [Header("///")]
    public float patrolStopInterval = 5f;   
    public float stopDuration = 1.5f;       
    private bool isPaused = false;
    private bool playerInSight = false;
    private float patrolTimer = 0f;

    public float forcePush = 5f;

    public GameObject enemyDeathEffect;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip dieSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

        if (isPaused)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetFloat("speed", 0f);
            return;
        }

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
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        RaycastHit2D wallInfo = Physics2D.Raycast(
            wallCheck.position,
            direction,
            wallDetectionRange,
            groundLayer
        );

        Debug.DrawRay(wallCheck.position, direction * wallDetectionRange, Color.red);

        if (wallInfo)
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

        isPaused = false;

        patrolTimer = 0f;
    }

    private void Flip()
    {
        facingRight = !facingRight;

        movement.x = facingRight ? 1f : -1f;

        transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                playerRb.AddForce(Vector2.up * forcePush, ForceMode2D.Impulse);

                if (audioSource != null && dieSound != null)
                {
                    AudioSource.PlayClipAtPoint(dieSound, transform.position);
                }

                if (enemyDeathEffect != null)
                {
                    Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0f);
                    Instantiate(enemyDeathEffect, spawnPos, Quaternion.identity); 
                }

                Destroy(gameObject, 0.3f);
            }
        }
    }
}