using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    
    int maxZombiesToSpawn;
    int spawned = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        StartSpawning(150, 0.2f);
    }
    
    void StartSpawning(int num, float delay)
    {
        spawned = 0;
        maxZombiesToSpawn = num;
        InvokeRepeating("SpawnZombie", 0, delay);
    }

    void StopSpawning()
    {
        CancelInvoke("SpawnZombie");
    }
    
    void SpawnZombie()
    {
        if (spawned == maxZombiesToSpawn)
        {
            StopSpawning();
            return;
        }
        
        GameObject spawnedZombie = Instantiate(zombiePrefab,
                                               (Vector2) gameObject.transform.position,
                                               Quaternion.identity);
        Zombie zombie = spawnedZombie.GetComponent<Zombie>();                                 
        zombie.SetRotationAngle(Random.Range(0, 360));
        spawned += 1;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
