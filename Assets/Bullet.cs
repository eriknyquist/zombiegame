using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioClip wallSound;
    public AudioClip zombieSound;
    
    AudioSource audioSource;
    ParticleSystem blood;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.gameObject;
        Zombie zombie = collided.GetComponent<Zombie>();
        
        // Did we hit a zombie?
        if (zombie != null)
        {
            audioSource.PlayOneShot(zombieSound, 0.2f);
            zombie.BulletHit();
        }
        else
        {
            audioSource.PlayOneShot(wallSound, 0.2f); 
        }
        
        /* Disable rigidbody and boxcollider, so the bullet effectively disappears,
         * but the sound will keep playing  */
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        rend.enabled = false;
        Destroy(bc);
        
        // Finally, destroy the bullet after 1s
        Destroy(gameObject, 1f);
    }
}
