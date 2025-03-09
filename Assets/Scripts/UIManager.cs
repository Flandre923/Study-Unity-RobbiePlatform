using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI orbText, time, deathText, gameOverText;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateOrbUI(int orbCount)
    {
        instance.orbText.text = orbCount.ToString();
    }

    public static void UpdateDeathUI(int deathCount)
    {
        instance.deathText.text = deathCount.ToString();    
    }

    public static void UpdateTimeUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);  
        int seconds = Mathf.FloorToInt(time % 60);
        
        instance.time.text = minutes.ToString("00") + ":" + seconds.ToString("00"); 
    }


    public static void DisplayGameOverUI()
    {
        instance.gameOverText.enabled = true;
    }
}
