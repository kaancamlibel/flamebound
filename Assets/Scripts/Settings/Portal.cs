using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;
    public GameObject portalEffect;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    public AudioClip teleportSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (destination != null)
            {
                if (audioSource != null && teleportSound != null)
                {
                    audioSource.PlayOneShot(teleportSound);
                }

                collision.transform.position = destination.position;
                StartCoroutine(PortalCooldown());

                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }

                Debug.Log("Iþýnlanma tamamlandý!");
            }
            else
            {
                Debug.LogWarning("Hedef (Destination) atanmamýþ!");
            }
        }
    }

    IEnumerator PortalCooldown()
    {
        portalEffect.SetActive(true);
        yield return new WaitForSeconds(2);
        portalEffect.SetActive(false);
    }
}