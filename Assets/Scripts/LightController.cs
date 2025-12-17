using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public float lightSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;

    public Transform playerLocation;
    public bool isLocked = true;

    public BoxCollider2D boundsCollider;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    public float radius = 3f;
    public float force = 10f;
    public LayerMask affectedLayers;

    public BoxCollider2D BoxCollider2D;

    private bool isCollidingWithPlayer;
    private PlayerController playerController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        // Initial calculation of bounds
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RequestLock();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyForce();
        }

        movement.x = Input.GetAxisRaw("LightHorizontal");
        movement.y = Input.GetAxisRaw("LightVertical");

        if (movement != Vector2.zero)
        {
            RequestFree();
        }
    }

    private void FixedUpdate()
    {
        CalculateBounds();

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
        BoxCollider2D.isTrigger = true;
    }

    void RequestFree()
    {
        isLocked = false;
        BoxCollider2D.isTrigger = false;
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
        if (playerController == null) return;
        if (playerController.isJumping) return;

        rb.velocity = movement.normalized * lightSpeed;

        ClampPosition();
    }

    void CalculateBounds()
    {
        Bounds b = boundsCollider.bounds;
        minBounds = b.min;
        maxBounds = b.max;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position; 

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x); 
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y); 

        transform.position = pos;
    }

    public void ApplyForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            affectedLayers
        );

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb == null) continue;

            Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }
}