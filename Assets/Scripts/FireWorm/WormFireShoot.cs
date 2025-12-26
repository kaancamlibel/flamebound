using UnityEngine;

public class WormFireShoot : MonoBehaviour
{
    public float speed = 1.0f;

    void Start()
    {
        Destroy(gameObject, 10);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Destroy(gameObject);
        }
    }
}
