using System;
using UnityEngine;
using UnityEngine.Events;

public class Barrel : MonoBehaviour
{
    public int maxHits = 10;
    public int hits = 10;

    public UnityEvent unityEvent;
    
    private void Start()
    {
        hits = maxHits;
    }

    public void TakeDamage()
    {
        hits = Mathf.Clamp(hits-1, 0, maxHits);
        
        if (hits <= 0) 
            Break();
    }

    private void Break()
    {
        unityEvent.Invoke();
    }
}