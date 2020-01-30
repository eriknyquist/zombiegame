using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos = gameObject.transform.position;
        camPos.x = player.transform.position.x;
        camPos.y = player.transform.position.y;
        gameObject.transform.position = camPos;
    }
}
