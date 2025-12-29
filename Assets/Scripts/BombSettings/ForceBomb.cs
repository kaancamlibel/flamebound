using System.Collections;
using UnityEngine;

public class ForceBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionVFX;
    public float delayBeforeExplosion = 1.5f; 
    public float explosionRadius = 3f;        
    public float shakeIntensity = 0.1f;       

    [Header("Layer Settings")]
    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    private bool isTriggered = false;
    private bool isExploded = false;
    private SpriteRenderer spriteRenderer;

    public System.Action OnBombDestroyed;

    [Header("Audio Settings")]
    public AudioClip explosionSound; 
    [Range(0, 1)] public float volume = 1f; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ExplodeByForce()
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(ShakeAndExplodeRoutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded) return;

        int colLayer = collision.gameObject.layer;

        if (((1 << colLayer) & enemyLayer) != 0 || ((1 << colLayer) & playerLayer) != 0)
        {
            InstantExplode();
        }
    }

    IEnumerator ShakeAndExplodeRoutine()
    {
        float elapsed = 0f;
        Vector3 originalLocalPos = transform.localPosition;

        while (elapsed < delayBeforeExplosion)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position += new Vector3(offsetX, offsetY, 0);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.white, Color.red, elapsed / delayBeforeExplosion);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        InstantExploteLogic();
    }

    public void InstantExplode()
    {
        if (isExploded) return;
        StopAllCoroutines(); 
        InstantExploteLogic();
    }

    private void InstantExploteLogic()
    {
        if (isExploded) return;
        isExploded = true;

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, volume);
        }

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 0.5f);
        }

        ApplyAreaDamage();

        Destroy(gameObject);
    }

    private void ApplyAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (((1 << hit.gameObject.layer) & enemyLayer) != 0)
            {
                Destroy(hit.gameObject);
            }
            else if (((1 << hit.gameObject.layer) & playerLayer) != 0)
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null) pc.health = 0;
            }
        }
    }

    private void OnDestroy()
    {
        if (OnBombDestroyed != null) OnBombDestroyed.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}