using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    int score = 0;
    
    // Offset from player's position
    Vector3 offset = new Vector3(2.0f, -3.5f, 0.0f);
    
    GameObject player;
    TextMesh textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = gameObject.GetComponent<TextMesh>();
        player = GameObject.FindGameObjectWithTag("Player");
        MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
        rend.sortingOrder = 100;
        
        SetScore(score);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position + offset;
    }
    
    public void IncrementScore()
    {
        score += 1;
        SetScore(score);
    }
    
    public void SetScore(int score)
    {
        textMesh.text = score.ToString();
    }
}
