using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collided = collision.gameObject;
        Zombie zombie = collided.GetComponent<Zombie>();
        
        // Did we hit a zombie?
        if (zombie != null)
        {
            // Decrement zombies health
            zombie.hp -= 1;
            
            if (zombie.hp == 0)
            {
                // Destroy zombie if its health is empty
                Destroy(collided);
            }
        }
        
        Destroy(gameObject);
    }
}
