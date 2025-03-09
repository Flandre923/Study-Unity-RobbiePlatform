using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private ScnenFader fader;
    private DoorScript doorScript;  
    private List<Orb> orbs;

    public int deathNumber;
    
    private float gameTime;
    private bool gameIsOver;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        orbs = new List<Orb>();
        
        DontDestroyOnLoad(gameObject);
    }

    public static void registerDoorScript(DoorScript door)
    {
        instance.doorScript = door;
    }
    public static void RegisterSceneFader(ScnenFader obj)
    {
        instance.fader = obj;
    }

    public static void RegisterOrb(Orb obj)
    {
        if (instance == null)
            return;
        if (!instance.orbs.Contains(obj))
            instance.orbs.Add(obj);
        UIManager.UpdateOrbUI(instance.orbs.Count);
        AudioManager.playerWonAudio();
    }
    
    
    public static void PlayerDied()
    {
        instance.fader.FadeOut();
        instance.deathNumber++;
        UIManager.UpdateDeathUI(instance.deathNumber);
        instance.Invoke("RestartScene",1.5f);        
    }

    public static void PlayerWon()
    {
        instance.gameIsOver = true;
        UIManager.DisplayGameOverUI();
        
    }

    public static bool GameOver()
    {
        return instance.gameIsOver;
    }
    public static void PlayerGrabbedOrb(Orb orb)
    {
        if (!instance.orbs.Contains(orb))
            return;
        instance.orbs.Remove(orb);

        if (instance.orbs.Count == 0)
            instance.doorScript.DoorUp();
        
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }

    void RestartScene()
    {
        instance.orbs.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (gameIsOver)
            return;
        gameTime += Time.deltaTime;
        UIManager.UpdateTimeUI(gameTime);
    }
    
    
    
}
