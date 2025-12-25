using UnityEngine;

public class SkullStraightMove : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 3f;

    private Animator animator;
    private bool isStopped = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Sahneyi kalabalýklaþtýrmamak için 3 saniye sonra kendini yok etsin
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isStopped)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("Burst");
            isStopped = true;

            // Oyuncuya çarparsa yok olsun
            Destroy(gameObject,0.2f);
        }
    }
}