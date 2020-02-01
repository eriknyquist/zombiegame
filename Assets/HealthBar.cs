using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Transform bar;
    
    // Start is called before the first frame update
    void Start()
    {
        bar = gameObject.transform.Find("Bar");
    }
    
    public void SetHealth(float health)
    {
        Vector2 scale = bar.transform.localScale;
        scale.x = health;
        bar.transform.localScale = scale;
    }
}
