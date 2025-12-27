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

    void Awake()
    {
        animator = GetComponent<Animator>();
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
    }

    private void FixedUpdate()
    {
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
        Debug.Log("Boss Hasar Aldý! Kalan Can: " + currentHealth);

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
        Debug.Log("Boss Öldü!");
        Destroy(gameObject);
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