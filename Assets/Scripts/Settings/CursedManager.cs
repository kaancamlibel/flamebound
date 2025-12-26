using UnityEngine;

public class CursedManager : MonoBehaviour
{
    public static CursedManager Instance;

    [Header("///")]
    public GameObject cursedControl;
    public GameObject cursedVFX;

    private void Awake()
    {
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