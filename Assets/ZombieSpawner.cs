using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    
    float spawnDelay = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnZombie", 0, spawnDelay);
    }

    void SpawnZombie()
    {
        Instantiate(zombiePrefab, (Vector2) gameObject.transform.position, Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
