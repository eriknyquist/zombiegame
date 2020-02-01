using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    GameObject player;
    Transform bar;
    
    // Offset from player's position
    Vector3 offset = new Vector3(0.0f, -3.8f, 0.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bar = gameObject.transform.Find("Bar");
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position + offset;
    }
    
    public void setHealth(float health)
    {
        Vector2 scale = bar.transform.localScale;
        scale.x = health;
        bar.transform.localScale = scale;
    }
}
