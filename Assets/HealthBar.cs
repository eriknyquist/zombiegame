using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    GameObject player;
    Transform bar;
    Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bar = gameObject.transform.Find("Bar");
        offset = gameObject.transform.position - player.transform.position;
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
