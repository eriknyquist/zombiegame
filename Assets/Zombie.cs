using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zombie : MonoBehaviour
{
    /* Enumeration of possible states a zombie can be in */
    enum State
    {
        IDLE,     // Randomly milling about
        TURNING,  // Turning to face player
        PURSUING, // Following player with line of sight
        TRACKING  // Moving toward player's last seen position
    }
    
    // Constant velocity damping value
    public float damping = 0.9f;
    
    // Number of times a zombie can be shot before dying
    public int hp = 2;
    
    // Reference to player gameobject
    Transform player;
    
    // Movement speed for IDLE state
    const float SLOW_SPEED = 0.008f;
    
    // Movement speeds for PURSUING and TRACKING states
    const float FAST_SPEED = 0.04f;
    
    LevelManager levelManager;
    PlayerHUD playerHUD;
    float zombieAngle;
    float turnStep;
    Vector2 playerPos;
    Vector2 lastSeenPlayerPos;
    RaycastHit2D playerHit;
    State state = State.IDLE;
    ParticleSystem blood;
    
    public void SetRotationAngle(float angle)
    {
        zombieAngle = angle;
    }
    
    public void BulletHit()
    {
        hp -= 1;
        
        EnableBlood();
        Invoke("DisableBlood", 0.2f);
        
        if (0 == hp)
        {
            Death();
        }
    }
    
    void DestroyZombie()
    {
        Destroy(gameObject);
    }
    
    void Death()
    {
        playerHUD.IncrementScore();
        
        /* Disable rigidbody and boxcollider, so the zombie effectively disappears,
         * but the particle system will keep emitting */
        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D bc = gameObject.GetComponent<BoxCollider2D>();
        rend.enabled = false;
        Destroy(bc);
        
        /* Destroy the gameobject, which will also destroy the particle system,
         * in 2 seconds */ 
        Invoke("DestroyZombie", 2f);
        
        // Inform LevelManager that a zombie was killed
        levelManager.ZombieKilled();
    }
    
    void EnableBlood()
    {
        blood.enableEmission = true;
    }
    
    void DisableBlood()
    {
        blood.enableEmission = false;
    }
    
    void Start()
    {
        GameObject hud = GameObject.FindGameObjectWithTag("PlayerHUD");
        playerHUD = hud.GetComponent<PlayerHUD>();

        GameObject mgr = GameObject.FindGameObjectWithTag("LevelManager");
        levelManager = mgr.GetComponent<LevelManager>();
        
        blood = gameObject.GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Save player position
        playerPos = player.position;
        
        // Save raycast towards player
        playerHit = Physics2D.Linecast(gameObject.transform.position, playerPos);
    }
    
    // Calculate angle of rotation required to make zombie look at given position
    float AngleTowardsPosition(Vector2 pos)
    {
        Vector2 currPos = gameObject.transform.position;
        Vector2 lookDir = pos - currPos;
        return Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    }
    
    // Rotate zombie to face given position
    void LookTowardsPosition(Vector2 pos)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, AngleTowardsPosition(pos));
    }
    
    // Move zombie forwards in the direction of rotation
    void MoveForwards(float speed)
    {
        gameObject.transform.position += gameObject.transform.right * speed;
    }
    
    void FixedUpdate()
    {        
        // Do we have line-of-sight to the player?
        bool haveLineOfSight = playerHit && ("Player" == playerHit.transform.name);
        
        if (haveLineOfSight)
        {
            // Update the position where we last saw the player
            lastSeenPlayerPos = player.position;
        }
        
        switch (state)
        {
            case State.PURSUING:
                if (!haveLineOfSight)
                {
                    state = State.TRACKING;
                    break;
                }
                
                // Turn zombie to face towards player
                LookTowardsPosition(playerPos);
                MoveForwards(FAST_SPEED);
                break;
                
            case State.TRACKING:
                if (haveLineOfSight)
                {
                    state = State.TURNING;
                    break;
                }
                
                if (Vector2.Distance(lastSeenPlayerPos, gameObject.transform.position) < 0.5f)
                {
                    state = State.IDLE;
                    break;
                }

                LookTowardsPosition(lastSeenPlayerPos);
                MoveForwards(FAST_SPEED);
                break;
                
            case State.IDLE:
                if (haveLineOfSight)
                {
                    state = State.TURNING;
                    turnStep = Random.Range(5f, 10f);
                    break;
                }
                
                // Turn zombie by some small random amount so it mills about randomly
                zombieAngle += Random.Range(-1.0f, 1.0f);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, zombieAngle);
                MoveForwards(SLOW_SPEED);
                break;
            
            case State.TURNING:
                float targetAngle = AngleTowardsPosition(player.position);
                float delta = Mathf.DeltaAngle(zombieAngle, targetAngle);
                
                if (Mathf.Abs(delta) < 10f)
                {
                    state = State.PURSUING;
                    break;
                }
                
                float step = turnStep;
                if (delta < 0f)
                {
                    step *= -1f;
                }
                
                zombieAngle += step;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, zombieAngle);
                break;
        }
        
        /* Constant velocity damping (otherwise zombies would fly off into eternity
         * whenever a bullet hits them) */
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity *= damping;
    }
}
