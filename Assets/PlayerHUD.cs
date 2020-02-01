using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    // Offset from player's position
    Vector3 offset = new Vector3(0.0f, -3.8f, 0.0f);
    
    Vector3 pivotOffset;
    GameObject player;
    HealthBar healthBar;
    ScoreBoard scoreBoard;

    // Start is called before the first frame update
    void Start()
    {
        GameObject centerPoint = gameObject.transform.Find("HUDCenterPoint").gameObject;
        pivotOffset = gameObject.transform.position - centerPoint.transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
        GameObject board = GameObject.FindGameObjectWithTag("ScoreBoard");
        scoreBoard = board.GetComponent<ScoreBoard>();
        GameObject bar = GameObject.FindGameObjectWithTag("HealthBar");
        healthBar = bar.GetComponent<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position + offset + pivotOffset;
    }
    
    public void SetHealth(float health)
    {
        healthBar.SetHealth(health);
    }
    
    public void SetScore(int score)
    {
        scoreBoard.SetScore(score);
    }
    
    public void IncrementScore()
    {
        scoreBoard.IncrementScore();
    }
}
