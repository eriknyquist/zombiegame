using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    public int max_ammo = 50;
    public int ammo;
    
    TextMesh textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = gameObject.GetComponent<TextMesh>();
        MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
        rend.sortingOrder = 100;
        
        Reload();
    }
    
    void DrawAmmoCount()
    {
        textMesh.text = ammo.ToString();
    }
    
    public void DecrementAmmo()
    {
        ammo -= 1;
        DrawAmmoCount();
    }
    
    public void Reload()
    {
        ammo = max_ammo;
        DrawAmmoCount();
    }
}
