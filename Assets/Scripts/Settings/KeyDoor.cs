using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    public int keysCollected = 0; 
    public int keysRequired = 2;  

    public void AddKey()
    {
        keysCollected++;
        Debug.Log("Anahtar Toplandý! Toplam: " + keysCollected);

        if (keysCollected >= keysRequired)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        Debug.Log("Kapý Açýldý!");
        Destroy(gameObject);
    }
}