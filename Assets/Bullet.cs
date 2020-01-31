using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    ParticleSystem blood;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.gameObject;
        Zombie zombie = collided.GetComponent<Zombie>();
        
        // Did we hit a zombie?
        if (zombie != null)
        {
            zombie.BulletHit();
        }
        
        Destroy(gameObject);
    }
}
