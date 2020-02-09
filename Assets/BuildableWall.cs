using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableWall : MonoBehaviour
{
    public int max_hp = 2;
    public int hp;
    
    // Start is called before the first frame update
    void Start()
    {
        hp = max_hp;
    }

    public void BulletHit()
    {
        hp -= 1;
        if (0 == hp)
        {
            Destroy(gameObject);
        }
    }
}
