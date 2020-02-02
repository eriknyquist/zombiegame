using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public AudioClip gunSound;

    AudioSource audioSource;
    PlayerHUD playerHUD;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject hud = GameObject.FindGameObjectWithTag("PlayerHUD");
        playerHUD = hud.GetComponent<PlayerHUD>();    
    }
    
    void FireBullet()
    {
        if (0 == playerHUD.ammoCounter.ammo)
        {
            return;
        }
        
        audioSource.PlayOneShot(gunSound, 0.5f);
        playerHUD.ammoCounter.DecrementAmmo();
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        Destroy(bullet, 2f);
    }
    
    // Update is called once per frame
    void Update()
    {
        bool pressing = Input.GetButtonDown("Fire1");
        
        if (Input.GetButtonDown("Fire1"))
        {
            InvokeRepeating("FireBullet", 0, 0.1f);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke("FireBullet");
        }
    }
}
