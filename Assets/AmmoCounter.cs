using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    int count = 0;
    
    TextMesh textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = gameObject.GetComponent<TextMesh>();
        MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
        rend.sortingOrder = 100;
        
        SetAmmoCount(count);
    }
    
    public void SetAmmoCount(int count)
    {
        textMesh.text = count.ToString();
    }
}
