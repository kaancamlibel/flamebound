using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Patlama Ayarlarý")]
    public GameObject explosionVFX;
    public float bombCooldown = 1.5f;
    public float explosionRadius = 3f;
    public float shakeIntensity = 0.1f;

    [Header("Katmanlar")]
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask bossLayer;

    private bool isExploded = false;
    private bool isTimerStarted = false;
    private SpriteRenderer spriteRenderer;

    [Header("Ses Ayarlarý")]
    public AudioClip explosionSound; 
    [Range(0, 1)] public float volume = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded) return;

        int colLayer = collision.gameObject.layer;

        if (((1 << colLayer) & bossLayer) != 0)
        {
            InstantExplode();
        }
        else if (((1 << colLayer) & enemyLayer) != 0 || ((1 << colLayer) & playerLayer) != 0)
        {
            InstantExplode();
        }
        else if (!isTimerStarted)
        {
            StartCoroutine(ShakeAndExplode());
        }
    }

    public void InstantExplode()
    {
        if (isExploded) return;
        isExploded = true;
        StopAllCoroutines();

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

    void ApplyAreaDamage()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            int layer = obj.gameObject.layer;

            if (((1 << layer) & bossLayer) != 0)
            {
                DemonBossAI boss = obj.GetComponent<DemonBossAI>();
                if (boss != null) boss.TakeDamage(1);
            }
            else if (((1 << layer) & enemyLayer) != 0)
            {
                Destroy(obj.gameObject);
            }
            else if (((1 << layer) & playerLayer) != 0)
            {
                PlayerController pc = obj.GetComponent<PlayerController>();
                if (pc != null) pc.health = 0;
            }
        }
    }

    IEnumerator ShakeAndExplode()
    {
        isTimerStarted = true;
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < bombCooldown)
        {
            transform.position += (Vector3)Random.insideUnitCircle * shakeIntensity;
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, elapsed / bombCooldown);
            elapsed += Time.deltaTime;
            yield return null;
        }
        InstantExplode();
    }

    public void ExplodeByForce() { InstantExplode(); }
}