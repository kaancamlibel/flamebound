using UnityEngine;

public class BossFightControl : MonoBehaviour
{
    public GameObject Boss;
    public GameObject tornadoSpawner;
    public GameObject lavaWalls1;
    public GameObject lavaWalls2;
    public GameObject randomBombSpawner;

    private bool hasTriggered = false;
    private bool isBossDefeated = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered && !isBossDefeated)
        {
            StartFight();
        }
    }

    public void StartFight()
    {
        hasTriggered = true;
        SetFightState(true);
    }

    public void OnBossDefeated()
    {
        isBossDefeated = true; 
        SetFightState(false);  
        Debug.Log("Savaþ Kalýcý Olarak Bitti.");
    }

    public void ResetFight()
    {
        if (isBossDefeated || Boss == null) return;

        hasTriggered = false;
        SetFightState(false);
    }

    private void SetFightState(bool state)
    {
        if (tornadoSpawner != null) tornadoSpawner.SetActive(state);
        if (lavaWalls1 != null) lavaWalls1.SetActive(state);
        if (lavaWalls2 != null) lavaWalls2.SetActive(state);
        if (randomBombSpawner != null) randomBombSpawner.SetActive(state);

        if (Boss != null)
        {
            Boss.SetActive(state);
        }

        if (state && Boss != null) Boss.SetActive(true);
    }
}