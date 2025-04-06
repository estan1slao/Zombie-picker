using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [NonSerialized] public GunData GunData; 
    
    private void Update()
    {
        if (transform.position.x >= 20)
            Destroy(gameObject);
    }
}