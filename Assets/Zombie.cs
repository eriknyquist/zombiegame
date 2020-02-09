using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zombie : MonoBehaviour
{
    /* Enumeration of possible states a zombie can be in */
    public enum State
    {
        IDLE,             // Randomly milling about
        PURSUING,         // Following player with line of sight
        PURSUING_BLINDLY, // Following a zombie that has line-of-sight to the player
        TRACKING_BLINDLY, // Following a zombie that is in the TRACKING state
        TRACKING          // Moving toward player's last seen position
    }
    
    
    /* Enumeration of possible results from a linecast from zombie to player */
    enum RaycastHitType
    {
        PLAYER_LOS,    // Zombie has line-of-sight to player
        PURSUING_LOS,  // Line-of-sight to player is being blocked by a pursuing zombie
        NO_LOS         // Can't see nuthin good
    }
    
    // Constant velocity damping value
    public float damping = 0.9f;
    
    // Number of times a zombie can be shot before dying
    public int hp = 2;
    
    public State state = State.IDLE;
    
    public bool killed;
    
    // Reference to player gameobject
    Transform player;
    
    // Movement speed for IDLE state
    const float SLOW_SPEED = 0.008f;
    
    // Movement speeds for PURSUING and TRACKING states
    const float FAST_SPEED = 0.04f;
    
    // Min. wall distance before zombie will turn away
    const float WALL_BOUNDARY = 1f;
    
    /* We will do a raycast towards the player's position, to see if we have line-of-sight,
     * every framesPerPlayerRaycast frames */
    const int framesPerPlayerRaycast = 30;
    
    /* When in the idle state, we will do a 180 degree raycast sweep in front of
     * the zombie to find the best direction to face every updatesPerRaycastSweep
     physics updates */
    const int updatesPerRaycastSweep = 30;
    
    int framesSinceLastCast;
    int updatesSinceLastSweep;
    BoxCollider2D boxCollider;
    LevelManager levelManager;
    PlayerHUD playerHUD;
    Vector2 playerPos;
    Vector2 lastSeenPlayerPos;
    RaycastHit2D playerHit;
    ParticleSystem blood;
    Quaternion idleLookDirection;
    
    GameObject buddyZombie;
    Zombie buddyZombieScript;
    RaycastHitType hitType = RaycastHitType.NO_LOS;
    
    public void SetRotationAngle(float angle)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
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
        killed = true;
        playerHUD.scoreBoard.IncrementScore();
        
        /* Disable rigidbody and boxcollider, so the zombie effectively disappears,
         * but the particle system will keep emitting */
        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
        rend.enabled = false;
        Destroy(boxCollider);
        
        // Inform LevelManager that a zombie was killed
        levelManager.ZombieKilled();
        
        /* Destroy the gameobject, which will also destroy the particle system,
         * in 1 second */ 
        Invoke("DestroyZombie", 1f);
    }
    
    void EnableBlood()
    {
        blood.enableEmission = true;
    }
    
    void DisableBlood()
    {
        blood.enableEmission = false;
    }
    
    /* Do multiple raycasts in a 180 degree sweep in front of the zombie, and
       calculate the rotation vector required to make the zombie look in the
       direction of whichever raycast had the highest distance. Helps mitigate
       zombies getting stuck on walls/corners (as opposed to just turning to a
       random direction) */
    Quaternion RaycastSweepForLookDirection()
    {
        // Return cached copy, no need to do a raycast sweep on every physics update
        if (updatesSinceLastSweep < updatesPerRaycastSweep)
        {
            updatesSinceLastSweep += 1;
            return idleLookDirection;
        }
        
        updatesSinceLastSweep = 0;
        
        // Total number of raycasts to do in the sweep
        const int numCasts = 6;
        
        // Number of degrees to increment rotation by after each cast
        float degreesIncrement = 180f / (float) numCasts;
        
        // Highest cast distance we've seen so far
        float highestDistance = 0f;
        
        // Rotation angle offset corresponding with highest cast distance
        float highestAngle = 0f;
        
        // Temporarily disable boxcollider so we don't hit ourselves with the raycast
        boxCollider.enabled = false;
        
        // Do a 180 degree sweep of raycasts
        for (float angleOffset = 0f; angleOffset <= 180f; angleOffset += degreesIncrement)
        {
            // Calculate direction for this raycast
            Vector3 castDir = Quaternion.Euler(0, 0, angleOffset) * (-transform.up);
            
            // Do the raycast
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, castDir);

            // Draw a line showing the raycast, uncomment for debugging
            Debug.DrawLine(gameObject.transform.position, hit.point, Color.green);
            
            if (hit.distance > highestDistance)
            {
                highestDistance = hit.distance;
                highestAngle = angleOffset;
            }
        }

        // Re-enable boxcollider
        boxCollider.enabled = true;
        
        // Calculate angle of rotation to face direction of cast with the highest distance
        idleLookDirection = gameObject.transform.rotation * Quaternion.Euler(0, 0, -90f) * Quaternion.Euler(0, 0, highestAngle);
        return idleLookDirection;
    }

    RaycastHitType TranslatePlayerLineCast(RaycastHit2D hit)
    {
        if (!hit)
        {
            return RaycastHitType.NO_LOS;
        }
        
        if ("Player" == hit.transform.gameObject.tag)
        {
            return RaycastHitType.PLAYER_LOS;
        }
        
        if ("Zombie" == hit.transform.gameObject.tag)
        {
            buddyZombie = hit.transform.gameObject;
            Zombie otherZombie = hit.transform.gameObject.GetComponent<Zombie>();
            if ((State.PURSUING == otherZombie.state) || (State.PURSUING_BLINDLY == otherZombie.state))
            {
                return RaycastHitType.PURSUING_LOS;
            }
        }
        
        return RaycastHitType.NO_LOS;
    }

    void UpdatePlayerLineCast()
    {
        if (framesSinceLastCast < framesPerPlayerRaycast)
        {
            framesSinceLastCast += 1;
        }
        else
        {
            playerHit = Physics2D.Linecast(gameObject.transform.position, playerPos);
            hitType = TranslatePlayerLineCast(playerHit);
            framesSinceLastCast = 0;
        }
    }
    
    void Start()
    {
        GameObject hud = GameObject.FindGameObjectWithTag("PlayerHUD");
        playerHUD = hud.GetComponent<PlayerHUD>();

        GameObject mgr = GameObject.FindGameObjectWithTag("LevelManager");
        levelManager = mgr.GetComponent<LevelManager>();
        
        blood = gameObject.GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        
        updatesSinceLastSweep = 0;
        framesSinceLastCast = 0;
        killed = false;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (killed)
        {
            /* If zombie has been killed and we're just waiting for the gameobject
             * to be destroyed, no need to do anything here */
            return;
        }
        
        // Save player position
        playerPos = player.position;
        
        // Temporarily disable boxcollider so we don't hit ourselves with the raycast
        boxCollider.enabled = false;
        
        // Update raycast towards player
        UpdatePlayerLineCast();
        
        // Re-enable boxcollider
        boxCollider.enabled = true;
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
        //gameObject.transform.rotation = Quaternion.Euler(0, 0, AngleTowardsPosition(pos));
        Quaternion newRot = Quaternion.Euler(0, 0, AngleTowardsPosition(pos));
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, newRot, 0.1f);
    }
    
    // Move zombie forwards in the direction of rotation
    void MoveForwards(float speed)
    {
        gameObject.transform.position += gameObject.transform.right * speed;
    }

    void FixedUpdate()
    {
        if (killed)
        {
            /* If zombie has been killed and we're just waiting for the gameobject
             * to be destroyed, no need to do anything here */
            return;
        }
        
        if (RaycastHitType.PLAYER_LOS == hitType)
        {
            // Update the position where we last saw the player
            lastSeenPlayerPos = player.position;
        }
        
        switch (state)
        {
            case State.PURSUING:
                if (RaycastHitType.PURSUING_LOS == hitType)
                {
                    state = State.PURSUING_BLINDLY;
                    break;
                }
                
                if (RaycastHitType.NO_LOS == hitType)
                {
                    state = State.TRACKING;
                    break;
                }
                
                // Turn zombie to face towards player
                LookTowardsPosition(playerPos);
                MoveForwards(FAST_SPEED);
                break;

            case State.PURSUING_BLINDLY:
                if (RaycastHitType.PLAYER_LOS == hitType)
                {
                    state = State.PURSUING;
                    break;
                }
                
                if (RaycastHitType.NO_LOS == hitType)
                {
                    //state = State.IDLE;
                    buddyZombieScript = buddyZombie.GetComponent<Zombie>();
                    state = State.TRACKING_BLINDLY;
                    break;
                }
                
                // Turn zombie to face towards player
                LookTowardsPosition(playerPos);
                MoveForwards(FAST_SPEED);
                break;

            case State.TRACKING_BLINDLY:
                if (buddyZombieScript.killed || (State.IDLE != buddyZombieScript.state))
                {
                    state = State.IDLE;
                    break;
                }

                if (RaycastHitType.PLAYER_LOS == hitType)
                {
                    state = State.PURSUING;
                    break;
                }
                
                if (RaycastHitType.PURSUING_LOS == hitType)
                {
                    state = State.PURSUING_BLINDLY;
                    break;
                }
                LookTowardsPosition(buddyZombie.transform.position);
                MoveForwards(FAST_SPEED);
                break;
                
            case State.TRACKING:
                if (RaycastHitType.PLAYER_LOS == hitType)
                {
                    state = State.PURSUING;
                    break;
                }
                
                if (RaycastHitType.PURSUING_LOS == hitType)
                {
                    state = State.PURSUING_BLINDLY;
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
                if (RaycastHitType.PLAYER_LOS == hitType)
                {
                    state = State.PURSUING;
                    break;
                }
                
                if (RaycastHitType.PURSUING_LOS == hitType)
                {
                    state = State.PURSUING_BLINDLY;
                    break;
                }
                
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation,
                                                                RaycastSweepForLookDirection(),
                                                                0.02f);
                MoveForwards(SLOW_SPEED);
                break;
        }
        
        /* Constant velocity damping (otherwise zombies would fly off into eternity
         * whenever a bullet hits them) */
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity *= damping;
    }
}
