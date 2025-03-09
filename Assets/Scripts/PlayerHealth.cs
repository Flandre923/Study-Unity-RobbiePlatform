using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    public GameObject deathVFXPrefab;
    
    private int trapsLayer;
    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("Traps");
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == trapsLayer)
        {
            // Instantiate(deathVFXPrefab, transform.position, transform.rotation);
            Instantiate(deathVFXPrefab,transform.position,Quaternion.Euler(0,0,Random.Range(-45,90)));
            gameObject.SetActive(false);
            
            AudioManager.PlayDeathAudio();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            GameManager.PlayerDied();
        }
    }
}
