using UnityEngine;

public class FireWormAI : MonoBehaviour
{
    [Header("Yol Noktalarý")]
    public Transform pointA;
    public Transform pointB;

    [Header("Hareket Ayarlarý")]
    public float speed = 3f;

    private Transform currentTarget;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Ýlk hedef olarak A noktasýný belirle
        currentTarget = pointA;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("FireWorm: Lütfen PointA ve PointB referanslarýný atayýn!");
        }
    }

    void Update()
    {
        if (currentTarget == null) return;

        // Hedefe doðru ilerle
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Hedefe ulaþtý mý kontrol et
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            FlipAndChangeTarget();
        }
    }

    void FlipAndChangeTarget()
    {
        // Hedefi deðiþtir
        if (currentTarget == pointA)
        {
            currentTarget = pointB;
        }
        else
        {
            currentTarget = pointA;
        }

        // Karakterin yüzünü döndür
        // Hareket yönü saða doðru ise flipX false, sola doðru ise true (Sprite'ýna göre deðiþebilir)
        if (currentTarget.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Saða bak
        }
        else
        {
            spriteRenderer.flipX = true; // Sola bak
        }
    }

    // Editörde rotayý görebilmek için
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