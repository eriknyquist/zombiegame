﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public HealthBar healthBar;
    public ScoreBoard scoreBoard;
    public AmmoCounter ammoCounter;
    public GameObject player;

    // Offset from player's position
    Vector3 offset = new Vector3(0.0f, -3.8f, 0.0f);
    
    Vector3 pivotOffset;

    // Start is called before the first frame update
    void Start()
    {
        GameObject centerPoint = gameObject.transform.Find("HUDCenterPoint").gameObject;
        pivotOffset = gameObject.transform.position - centerPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position + offset + pivotOffset;
    }
}
