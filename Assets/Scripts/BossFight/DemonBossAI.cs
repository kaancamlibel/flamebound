using System.Collections;
using UnityEngine;

public class DemonBossAI : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform player;
    public float speed = 2f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("Prefabs")]
    private bool canAttack = true; 
    private bool isAttacking = false;
    public Transform firePoint;
    public GameObject fireBallPrefab;

    public GameObject dustPrefab; 
    public Transform dustSpawnPoint; 
    private float lastRotationY;

    private Animator animator;
    private PlayerController playerController;

    [Header("Boss Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip attackSound;
    public AudioClip fireballSound;
    public AudioClip deathSound;
    public AudioClip hurtSound;

    private float walkStepTimer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerController = p.GetComponent<PlayerController>();

        StartCoroutine(FireballRoutine(3, 1f, 10));

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!isAttacking && canAttack)
        {
            CheckForAttack();
        }

        ChangeRotation();
        PlayWalkingSound();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            animator.SetBool("isMoving", false); 
            return;
        }

        if (!isAttacking)
        {
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            animator.SetBool("isMoving", true);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                animator.SetBool("isMoving", false);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void PlayWalkingSound()
    {
        if (animator.GetBool("isMoving") && !isAttacking && !isDead)
        {
            walkStepTimer -= Time.deltaTime;
            if (walkStepTimer <= 0)
            {
                if (walkSound != null) audioSource.PlayOneShot(walkSound, 0.5f);
                walkStepTimer = 0.5f; // Adým sesleri arasý süre
            }
        }
    }

    void CheckForAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (hit != null)
        {
            StartCoroutine(PerformAttack());
        }
    }

    void DealDamage()
    {
        if (attackSound != null) audioSource.PlayOneShot(attackSound);

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (hit != null)
        {
            PlayerController pc = hit.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.health--;

                StartCoroutine(HitStop(0.12f));

                float dirX = pc.transform.position.x - transform.position.x;
                Vector2 knockbackDir = new Vector2(Mathf.Sign(dirX), 0);

                pc.StartCoroutine(pc.ApplyKnockback(knockbackDir));

                Debug.Log("Boss vurdu ve oyuncu savruldu!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitFlash());
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        canAttack = false;

        animator.SetBool("isMoving", false);
        animator.SetBool("Died", true);

        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        BossFightControl controller = FindObjectOfType<BossFightControl>();
        if (controller != null) controller.OnBossDefeated();

        Destroy(gameObject, 2f);
    }

    public void ResetBoss()
    {
        isDead = false;
        canAttack = true;
        isAttacking = false;
        currentHealth = maxHealth;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // Animasyonlarý sýfýrla
        animator.SetBool("Died", false);
        animator.SetBool("isMoving", false);
        animator.Rebind(); 
        animator.Update(0f);

        StopAllCoroutines();
        StartCoroutine(FireballRoutine(3, 1f, 10));

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator HitFlash()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration); 
        Time.timeScale = 1f;
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;     
        isAttacking = true;    

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1.5f);

        isAttacking = false;   

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;      
    }

    IEnumerator FireballRoutine(int count, float delayBetweenShots, float timeBetweenWaves)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

                isAttacking = true;
                yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < count; i++)
                {
                    SpawnFireBall();
                    yield return new WaitForSeconds(delayBetweenShots);
                }

                isAttacking = false;
        }
    }

    void SpawnFireBall()
    {
        if (fireBallPrefab != null)
        {
            if (fireballSound != null) audioSource.PlayOneShot(fireballSound);

            Vector3 spawnPosition = firePoint.position + new Vector3(0, 1f, 0);
            Instantiate(fireBallPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void ChangeRotation()
    {
        if (isAttacking) return;

        float distanceX = player.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > 0.1f)
        {
            lastRotationY = transform.eulerAngles.y;

            if (distanceX < 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            if (transform.eulerAngles.y != lastRotationY)
            {
                SpawnDust();
            }
        }
    }

    void SpawnDust()
    {
        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, dustSpawnPoint.position, transform.rotation);

            Destroy(dust, 0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}