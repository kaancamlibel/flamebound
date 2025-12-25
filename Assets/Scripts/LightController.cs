using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public float lightSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;

    public Transform playerLocation;
    public bool isLocked = true;

    public BoxCollider2D boundsCollider;
    public CircleCollider2D circleCollider2D;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    public float lockMargin = 0.5f;

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
        if (isKidnapped) return; // Kaçýrýlma varsa hiçbir inputu okuma

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
            freeMoveTimer = 2f; // Süreyi her hareketle tazele
        }
        else if (!isLocked)
        {
            // Hareket yoksa süreyi düþür
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

    void ApplyForce()
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
            rb.AddForce(direction * currentForce, ForceMode2D.Impulse);
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

    // Bu fonksiyon çaðrýldýðýnda ýþýk artýk oyuncuyu veya inputu dinlemez
    public void StartKidnapping(Vector2 targetPos, float moveSpeed)
    {
        isKidnapped = true; // Diðer tüm mantýðý devre dýþý býrak
        isLocked = false; // Takibi býrak
        StopAllCoroutines(); // Mevcut FreeMove geri sayýmlarýný durdur
        StartCoroutine(MoveToFinalPoint(targetPos, moveSpeed));
    }

    IEnumerator MoveToFinalPoint(Vector2 target, float speed)
    {
        // Iþýk hedef savaþ alanýna varana kadar hareket etsin
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            // Rigidbody üzerinden hareket ettirerek fizik kurallarýna (bounds vb.) uymasýný saðla
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }

        // Hedefe vardýðýnda orada çakýlý kalmasý için hýzý sýfýrla ve kilitle
        rb.velocity = Vector2.zero;
        isLocked = true; // Oyuncuya kilitlenmesi için deðil, hareketin durmasý için
                         // Not: FollowPlayer metoduna "if(kidnapped) return;" ekleyerek takibi tamamen kapatabilirsin.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKidnapped) return; // Kaçýrýlan ýþýða çarpýnca kilitlenme olmasýn

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