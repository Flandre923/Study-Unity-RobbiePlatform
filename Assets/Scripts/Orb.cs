using System;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private int player;
    public GameObject explosionVFXPrefab;
    
    void Start()
    {
        player = LayerMask.NameToLayer("Player");
        GameManager.RegisterOrb(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Instantiate(explosionVFXPrefab, transform.position, transform.rotation);
            
            gameObject.SetActive(false);
            
            AudioManager.PlayOrbAudio();
            GameManager.PlayerGrabbedOrb(this);
        }
    }
}
