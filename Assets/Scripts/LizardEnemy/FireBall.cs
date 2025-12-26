using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 6f;
    private Vector2 moveDir;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;
    }
}