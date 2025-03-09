using UnityEngine;

public class WineZoneScript : MonoBehaviour
{
    private int player;
    void Start()
    {
        player = LayerMask.NameToLayer("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Won!");
            GameManager.PlayerWon();
        }
    }
}
