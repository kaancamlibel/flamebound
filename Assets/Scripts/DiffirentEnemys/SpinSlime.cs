using UnityEngine;
using System.Collections;

public class SpinSlime : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float waitTime = 1f;

    private Transform currentTarget;
    private Animator animator;
    private bool isWaiting = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentTarget = pointA;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Lütfen Point A ve Point B alanlarýný doldurun!");
        }
        else
        {
            animator.SetBool("isSpinning", true);
        }
    }

    void Update()
    {
        if (isWaiting || currentTarget == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        animator.SetBool("isSpinning", false); 

        yield return new WaitForSeconds(waitTime); 

        SwitchTarget(); 

        isWaiting = false;
        animator.SetBool("isSpinning", true); 
    }

    void SwitchTarget()
    {
        currentTarget = (currentTarget == pointA) ? pointB : pointA;

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