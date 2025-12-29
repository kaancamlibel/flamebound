using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHopAttack : MonoBehaviour
{
    [Header("///")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;

    [Header("Main Settings")]
    public bool isAttacking;
    public float speed = 5f;
    bool isOnCooldown = false;

    private EnemySimpleHopAI hopAI;

    [Header("///")]
    public GameObject fireBall;
    private Vector2 moveDir;
    private Animator animator;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip attackSound;

    void Awake()
    {
        hopAI = GetComponent<EnemySimpleHopAI>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        animator.enabled = false;
    }

    private void Update()
    {
        CheckPlayer();

        transform.Translate(moveDir * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;
    }

    void CheckPlayer()
    {
        if (isOnCooldown) return;

        Vector2 direction = hopAI.facingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            detectionRange,
            playerLayer
        );

        Debug.DrawRay(transform.position, direction * detectionRange, Color.blue);

        if (hit.collider != null)
        {
            isAttacking = true;

            StartCoroutine(CoolDown());
        }

        else
        {
            isAttacking = false;
        }
    }
    IEnumerator CoolDown()
    {
        animator.enabled = true;
        isOnCooldown = true;

        yield return new WaitForSeconds(1.5f);

        isOnCooldown = false;
        animator.enabled = false;

    }

    void FireBall()
    {
        if (!isAttacking) return;

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        Vector2 shootDir = hopAI.facingRight ? Vector2.right : Vector2.left;
        GameObject bullet = Instantiate(fireBall, transform.position, Quaternion.identity);

        bullet.GetComponent<FireBall>().SetDirection(shootDir);
    }
}