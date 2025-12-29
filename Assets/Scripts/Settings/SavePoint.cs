using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private PlayerController pc;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    public AudioClip saveSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerInRange && pc != null && Input.GetKeyDown(KeyCode.E))
        {
            SaveGame();

            pc.health = 3;
            Debug.Log("Can Yenilendi: " + pc.health);
        }
    }

    void SaveGame()
    {
        if (audioSource != null && saveSound != null)
        {
            audioSource.PlayOneShot(saveSound);
        }

        PlayerPrefs.SetFloat("CheckPointX", pc.transform.position.x);
        PlayerPrefs.SetFloat("CheckPointY", pc.transform.position.y);
        PlayerPrefs.SetInt("HasCheckPoint", 1);

        PlayerPrefs.Save();
        Debug.Log("Kayýt Noktasý Alýndý!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            pc = collision.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            pc = null;
        }
    }
}