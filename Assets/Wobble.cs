using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    float speed = 10f;
    float amplitude = 0.1f;

    float startY;
    
    void Start()
    {
        startY = gameObject.transform.position.y;
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 currPos = gameObject.transform.position;
        currPos.y = startY + (Mathf.Sin(Time.time * speed) * amplitude);
        gameObject.transform.position = currPos;
    }
}
