using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    int level = 0;  // current level
    int kills = 0;  // number of kills so far in this level
    
    object[][] LevelData = new object[][] {
        //            Total zombie    Delay between
        //            spawns          zombie spawns
        new object[] {20,             0.5f},
        new object[] {40,             0.5f},
        new object[] {100,            0.2f},
        new object[] {200,            0.2f},
        new object[] {300,            0.2f},
        new object[] {400,            0.2f},
    };
    
    ZombieSpawner spawner1;
    ZombieSpawner spawner2;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject s1 = GameObject.Find("ZombieSpawnPoint1");
        GameObject s2 = GameObject.Find("ZombieSpawnPoint2");
        spawner1 = s1.GetComponent<ZombieSpawner>();
        spawner2 = s2.GetComponent<ZombieSpawner>();
        
        int numZombies = (int)LevelData[0][0];
        float zombieDelay = (float)LevelData[0][1];
        
        spawner1.StartSpawning(numZombies / 2, zombieDelay);
        spawner2.StartSpawning(numZombies / 2, zombieDelay);
    }

    public void ZombieKilled()
    {
        kills += 1;
        
        // Was the last zombie killed?
        if ((int)LevelData[level][0] == kills)
        {
            LevelUp();
        }
    }
    
    public void LevelUp()
    {
        level = (level + 1) % LevelData.Length;
    
        int numZombies = (int)LevelData[level][0];
        float zombieDelay = (float)LevelData[level][1];
        
        spawner1.StartSpawning(numZombies / 2, zombieDelay);
        spawner2.StartSpawning(numZombies / 2, zombieDelay);
        
        kills = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
