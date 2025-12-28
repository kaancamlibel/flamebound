using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Movement controls")]
    public float lightSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;

    [Header("Bounds controls")]
    public BoxCollider2D boundsCollider;
    public CircleCollider2D circleCollider2D;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    public float lockMargin = 0.5f;

    public Transform playerLocation;
    public bool isLocked = true;

    [Header("Force controls")]
    public float radius = 3f;
    public float force = 10f;
    private bool canForce = true;
    private float currentForce;
    public LayerMask affectedLayers;

    private bool isCollidingWithPlayer;
    private float freeMoveTimer;

    public bool isKidnapped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentForce = force;
    }

    private void Update()
    {
        if (isKidnapped) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RequestLock();
        }

        if (Input.GetKeyDown(KeyCode.F) && canForce)
        {
            ApplyForce();
            StartCoroutine(ForceDelay());
        }

        movement.x = Input.GetAxisRaw("LightHorizontal");
        movement.y = Input.GetAxisRaw("LightVertical");

        if (movement != Vector2.zero)
        {
            RequestFree();
            freeMoveTimer = 2f; 
        }
        else if (!isLocked)
        {
            freeMoveTimer -= Time.deltaTime;
            if (freeMoveTimer <= 0)
            {
                RequestLock();
            }
        }

    }

    private void FixedUpdate()
    {
        CalculateBounds();

        if (isKidnapped) return;

        CheckOutOfBounds();

        if (isLocked)
        {
            ApplyLockPhysics();
            FollowPlayer();
        }
        else
        {
            ApplyFreePhysics();
            FreeMove();
        }
    }

    void RequestLock()
    {
        isLocked = true;
        isCollidingWithPlayer = false;
        circleCollider2D.isTrigger = true;
    }

    void RequestFree()
    {
        isLocked = false;
        circleCollider2D.isTrigger = false;
    }

    void FollowPlayer()
    {
        Vector2 target = playerLocation.position;

        target.x = Mathf.Clamp(target.x, minBounds.x, maxBounds.x);
        target.y = Mathf.Clamp(target.y, minBounds.y, maxBounds.y);

        rb.velocity = Vector2.zero;
        rb.MovePosition(target);
    }

    void FreeMove()
    {
        if (isCollidingWithPlayer) return;

        rb.velocity = movement.normalized * lightSpeed;
    }

    void CalculateBounds()
    {
        Bounds b = boundsCollider.bounds;
        minBounds = b.min;
        maxBounds = b.max;
    }

    void CheckOutOfBounds()
    {
        Vector2 pos = rb.position;

        bool outOfBounds =
            pos.x < minBounds.x - lockMargin ||
            pos.x > maxBounds.x + lockMargin ||
            pos.y < minBounds.y - lockMargin ||
            pos.y > maxBounds.y + lockMargin;

        if (outOfBounds && !isLocked)
        {
            RequestLock();
        }
    }

    public void ResetLight()
    {
        StopAllCoroutines();

        isKidnapped = false;
        isLocked = true;
        isCollidingWithPlayer = false;
        circleCollider2D.isTrigger = true; 

        if (playerLocation != null)
        {
            transform.position = playerLocation.position;
        }

        rb.velocity = Vector2.zero;
        canForce = true; 

        Debug.Log("Iþýk Oyuncuya Geri Döndü!");
    }

    void ApplyForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            affectedLayers
        );

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rbHit = hit.attachedRigidbody;
            if (rbHit == null) continue;

            Vector2 direction = (rbHit.position - (Vector2)transform.position).normalized;
            rbHit.AddForce(direction * currentForce, ForceMode2D.Impulse);

            ForceBomb fBomb = hit.GetComponent<ForceBomb>();
            if (fBomb != null)
            {
                fBomb.ExplodeByForce();
            }
        }
    }
    IEnumerator ForceDelay()
    {
        canForce = false;
        yield return new WaitForSeconds(3f);
        canForce = true;
    }

    IEnumerator LockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RequestLock();
    }

    void ApplyLockPhysics()
    {
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void ApplyFreePhysics()
    {
        if (isCollidingWithPlayer)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX
                           | RigidbodyConstraints2D.FreezePositionY
                           | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void StartKidnapping(Vector2 targetPos, float moveSpeed)
    {
        isKidnapped = true; 
        isLocked = false;
        StopAllCoroutines(); 
        StartCoroutine(MoveToFinalPoint(targetPos, moveSpeed));
    }

    IEnumerator MoveToFinalPoint(Vector2 target, float speed)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isLocked = true; 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKidnapped) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;

            currentForce = force * 2;
            ApplyForce();

            StartCoroutine(LockAfterDelay(0.3f));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;

            currentForce = force;
        }
    }
}