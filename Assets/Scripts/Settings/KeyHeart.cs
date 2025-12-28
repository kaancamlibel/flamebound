using UnityEngine;

public class KeyHeart : MonoBehaviour
{
    [Header("Door Settings")]
    public KeyDoor keyDoor;

    [Header("Hover Settings")]
    public float amplitude = 0.5f; 
    public float frequency = 1f;   

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        if (keyDoor == null)
        {
            keyDoor = FindObjectOfType<KeyDoor>();
        }
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (keyDoor != null)
            {
                keyDoor.AddKey();
                Debug.Log("Anahtar Alýndý!");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("KeyDoor bulunamadý! Lütfen kapýyý atayýn.");
            }
        }
    }
}