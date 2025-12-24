using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionVFX;
    public float bombCooldown = 1f;
    public float explosionCooldown = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            StartCoroutine(BombCooldown());
        }
    }

    IEnumerator BombCooldown()
    {
        yield return new WaitForSeconds(bombCooldown);

        explosionVFX.SetActive(true);

        yield return new WaitForSeconds(explosionCooldown);

        Destroy(gameObject);
    }
}