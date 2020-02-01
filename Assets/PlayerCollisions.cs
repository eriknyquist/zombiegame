using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{
    public int coolOffSeconds = 1;
    public float coolOffFlashDelay = 0.1f;
    
    PlayerHUD playerHUD;
    int max_health = 5;
    SpriteRenderer rend;
    int health;
    bool coolingOff = false;
    
    /* Tracks the number of zombies touching us-- we increment in OnCollisionEnter
     * and decrement in OnCollisionExit */
    int touching = 0;
    
    void Start()
    {
        GameObject hud = GameObject.FindGameObjectWithTag("PlayerHUD");
        playerHUD = hud.GetComponent<PlayerHUD>();
        
        health = max_health;
        rend = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void EnterCoolOff()
    {
        coolingOff = true;
        InvokeRepeating("toggleRenderer", 0, coolOffFlashDelay);
    }
    
    void ExitCoolOff()
    {
        CancelInvoke("toggleRenderer");
        coolingOff = false;
        
        // make sure player rendering is enabled
        rend.enabled = true;
        
        /* Finally, check if there are currently zombies touching us, in which
         * case we'll trigger another zombie hit immediately */
         if (touching > 0)
         {
            PlayerHitByZombie();
         }
    }
    
    void toggleRenderer()
    {
        rend.enabled = !rend.enabled;
    }
  
    void Death()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);   
    }
    
    void PlayerHitByZombie()
    {
        // Decrement player's health
        health -= 1;
        playerHUD.SetHealth((float) health / (float) max_health);

        if (health == 0)
        {
            // Player is dead
            Death();
        }
        else
        {
            EnterCoolOff();
            Invoke("ExitCoolOff", coolOffSeconds);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        GameObject collided = collision.gameObject;
        Zombie zombie = collided.GetComponent<Zombie>();
        
        if (zombie != null)
        {
            // Exiting collision with a zombie
            touching -= 1;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.gameObject;
        Zombie zombie = collided.GetComponent<Zombie>();
        
        if (zombie != null)
        {
            // The thing we collided with was a zombie
            touching += 1;
            
            if (!coolingOff)
            {
                // Finished cooling off from last zombie hit
                PlayerHitByZombie();
            }
        }        
    }
}
