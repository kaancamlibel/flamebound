using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    [Header("UI Kalp Listesi")]
    public List<Image> hearts; // Inspector'dan 3 kalbi buraya sürükle

    // Bu fonksiyon PlayerController'dan gelen can miktarýna göre kalpleri açýp kapatýr
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            // Eðer index can miktarýndan küçükse kalbi göster, deðilse gizle
            if (i < currentHealth)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }
}