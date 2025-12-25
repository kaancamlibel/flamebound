using UnityEngine;
using System.Collections;

public class SpinSlime : MonoBehaviour
{
    [Header("Hedef Noktalar")]
    public Transform pointA;
    public Transform pointB;

    [Header("Hareket Ayarlarý")]
    public float speed = 10f;
    public float waitTime = 1f; // Bekleme süresi (1 saniye)

    private Transform currentTarget;
    private Animator animator;
    private bool isWaiting = false; // Bekleme durumunu kontrol eder

    void Start()
    {
        animator = GetComponent<Animator>();

        // Baþlangýçta A noktasýna gitmeye baþla
        currentTarget = pointA;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Lütfen Point A ve Point B alanlarýný doldurun!");
        }
        else
        {
            // Ýlk harekete baþlarken animasyonu aç
            animator.SetBool("isSpinning", true);
        }
    }

    void Update()
    {
        // Eðer bekleme modundaysak veya hedef yoksa hareket etme
        if (isWaiting || currentTarget == null) return;

        // Hedefe doðru hareket et
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Hedefe ulaþtý mý kontrol et
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        animator.SetBool("isSpinning", false); // Beklerken dönmeyi durdur

        yield return new WaitForSeconds(waitTime); // 1 saniye bekle

        SwitchTarget(); // Hedefi deðiþtir

        isWaiting = false;
        animator.SetBool("isSpinning", true); // Tekrar harekete geçerken dönmeyi baþlat
    }

    void SwitchTarget()
    {
        currentTarget = (currentTarget == pointA) ? pointB : pointA;

        // Yönü döndür (X ekseninde)
        Vector3 scaler = transform.localScale;
        if (currentTarget.position.x > transform.position.x)
        {
            scaler.x = Mathf.Abs(scaler.x);
        }
        else
        {
            scaler.x = -Mathf.Abs(scaler.x);
        }
        transform.localScale = scaler;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }
    }
}