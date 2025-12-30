using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public AudioSource menuAudioSource;
    public AudioClip clickSound;

    public void PlayGame()
    {
        if (menuAudioSource != null && clickSound != null)
        {
            menuAudioSource.PlayOneShot(clickSound);
        }

        StartCoroutine(LoadSceneWithDelay("Level1", 0.2f));
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}