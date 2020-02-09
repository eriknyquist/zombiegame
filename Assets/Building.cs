using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject wallPrefab;
    public Camera cam;
    
    void Start()
    {
   
    }
    
    void SpawnWall(Vector3 position)
    {
        // Make sure z position is zero'd out
        position.z = 0;
        
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {        
        if (Input.GetMouseButtonDown(1))
        {
            SpawnWall(cam.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
