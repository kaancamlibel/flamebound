using UnityEngine;

public class BossFightControl : MonoBehaviour
{
    public GameObject Boss;
    public GameObject tornadoSpawner;
    public GameObject lavaWalls1;
    public GameObject lavaWalls2;
    public GameObject randomBombSpawner;

    [Header("Müzik Ayarlarý")]
    public MusicPlaylistLoop backgroundPlaylist;
    public AudioClip bossMusic; 
    private AudioSource audioSource;

    private bool hasTriggered = false;
    private bool isBossDefeated = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.loop = true;

        if (backgroundPlaylist == null)
            backgroundPlaylist = FindObjectOfType<MusicPlaylistLoop>();
    }

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

        if (backgroundPlaylist != null) backgroundPlaylist.PauseMusic();

        if (audioSource != null && bossMusic != null)
        {
            audioSource.clip = bossMusic;
            audioSource.Play(); 
        }

        if (Boss != null)
        {
            Boss.SetActive(true);
            Boss.GetComponent<DemonBossAI>().ResetBoss();
        }
    }

    public void OnBossDefeated()
    {
        isBossDefeated = true;
        SetFightState(false);

        if (audioSource != null) audioSource.Stop(); 
        if (backgroundPlaylist != null) backgroundPlaylist.ResumeMusic();

        Debug.Log("Savaþ Kalýcý Olarak Bitti.");
    }

    public void ResetFight()
    {
        if (isBossDefeated) return;

        hasTriggered = false;
        SetFightState(false);

        if (audioSource != null) audioSource.Stop();
        if (backgroundPlaylist != null) backgroundPlaylist.ResumeMusic();

        if (Boss != null) Boss.SetActive(false);
    }

    private void SetFightState(bool state)
    {
        if (tornadoSpawner != null) tornadoSpawner.SetActive(state);
        if (lavaWalls1 != null) lavaWalls1.SetActive(state);
        if (lavaWalls2 != null) lavaWalls2.SetActive(state);
        if (randomBombSpawner != null) randomBombSpawner.SetActive(state);
    }
}