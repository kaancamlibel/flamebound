using UnityEngine;

public class FireWormAttack : MonoBehaviour
{
    public GameObject projectilePrefab; 
    private Animator animator;
    public Transform attackPoint;

    void Start() 
    {
        animator = GetComponent<Animator>();

        InvokeRepeating("TriggerAttack", 2f, 3f);
    }

    public void Shoot()
    {
        Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);
    }

    void TriggerAttack() 
    {
        animator.SetTrigger("Attack");
    }
}