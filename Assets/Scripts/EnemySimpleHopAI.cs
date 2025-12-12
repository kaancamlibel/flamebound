using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySimpleHopAI : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 0.1f;
    public int jumpFrameIndex = 2;
    public float holdDuration = 0.3f;
    public float frameWaitTime = 0.2f;

    public float jumpForce = 8f;
    public float moveForward = 2f;

    public Transform groundCheck;
    public float detectionRange = 0.5f;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public float wallDetectionRange = 0.5f;
    private bool facingRight;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private int currentFrame;
    private float timer;
    private bool isHolding;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        facingRight = true;
    }

    void Update()
    {
        if (isHolding)
        {
            sr.sprite = frames[currentFrame];
            return;
        }

        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
                currentFrame = 0;

            sr.sprite = frames[currentFrame];

            if (currentFrame == jumpFrameIndex)
            {
                DoJump();
            }
        }

        CheckWall();

    }

    void DoJump()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(
        groundCheck.position,
        Vector2.down,
        detectionRange,
        groundLayer
        );

        Debug.DrawRay(groundCheck.position, Vector2.down * detectionRange, Color.red);

        if (groundInfo)
        {
            rb.velocity = new Vector2(moveForward, jumpForce);
            isHolding = true;

            StartCoroutine(HoldOnJumpFrame());
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

    IEnumerator HoldOnJumpFrame()
    {
        yield return new WaitForSeconds(frameWaitTime);

        currentFrame = 0; // Reset to first frame after jump
        sr.sprite = frames[currentFrame];

        yield return new WaitForSeconds(holdDuration);
        isHolding = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;

        moveForward = -moveForward;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}