using UnityEngine;

public class CursedManager : MonoBehaviour
{
    // Singleton yapýsý: Her yerden kolayca eriþmek için
    public static CursedManager Instance;

    public GameObject cursedControl; // Aktif edilecek obje
    public GameObject cursedVFX;

    private void Awake()
    {
        // Bu scriptin her yerden "CursedManager.Instance" diye çaðrýlmasýný saðlar
        if (Instance == null) Instance = this;
    }

    public void StartCurse()
    {
        if (cursedControl != null)
        {
            cursedControl.SetActive(true);
            cursedVFX.SetActive(true);
            Debug.Log("Lanet Yönetici tarafýndan baþlatýldý!");
        }
    }
}