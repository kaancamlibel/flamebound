using UnityEngine;

public class FireWormAI : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 3f;

    private Transform currentTarget;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentTarget = pointA;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("FireWorm: Lütfen PointA ve PointB referanslarýný atayýn!");
        }
    }

    void Update()
    {
        if (currentTarget == null) return;

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            FlipAndChangeTarget();
        }
    }

    void FlipAndChangeTarget()
    {
        if (currentTarget == pointA)
        {
            currentTarget = pointB;
        }
        else
        {
            currentTarget = pointA;
        }

        if (currentTarget.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; 
        }
        else
        {
            spriteRenderer.flipX = true; 
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}