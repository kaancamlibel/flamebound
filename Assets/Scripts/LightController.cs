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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Initialize bounds
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isLocked = true;
        }

        movement.x = Input.GetAxisRaw("LightHorizontal");
        movement.y = Input.GetAxisRaw("LightVertical");

        if (movement != Vector2.zero)
        {
            isLocked = false;
        }
    }

    private void FixedUpdate()
    {
        CalculateBounds();

        if (isLocked)
        {
            FollowPlayer();
        }
        else
        {
            FreeMove();
        }
    }

    void FollowPlayer()
    {
        rb.velocity = Vector2.zero;
        rb.MovePosition(playerLocation.position);

        ClampPosition();
    }

    void FreeMove()
    {
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
}
