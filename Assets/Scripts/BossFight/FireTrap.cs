using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    public GameObject firePrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FireCooldown());
        }
    }

    IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(2f);
        firePrefab.SetActive(true);
        Debug.Log("Fire Trap Activated!");

        yield return new WaitForSeconds(4f);
        firePrefab.SetActive(false);

    }
}
