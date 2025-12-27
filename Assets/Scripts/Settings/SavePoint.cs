using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SaveGame();
        }
    }

    void SaveGame()
    {
        PlayerPrefs.SetFloat("CheckPointX", transform.position.x);
        PlayerPrefs.SetFloat("CheckPointY", transform.position.y);
        PlayerPrefs.SetInt("HasCheckPoint", 1);

        PlayerPrefs.Save();

        Debug.Log("Kayýt Noktasý Alýndý!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}