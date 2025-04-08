using System;
using UnityEngine;
using UnityEngine.Events;

public class Barrel : MonoBehaviour
{
    [NonSerialized] private int hits = 10;
    
    public int maxHits = 10;
    public float damage = 100f;

    public GameObject barrelMain;

    public UnityEvent breakEvent;
    
    private void Start()
    {
        hits = maxHits;
    }

    private void Update()
    {
        if (transform.position.x >= 20)
            Destroy(barrelMain);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Clone>().TakeDamage(damage);
        }
    }

    public void TakeDamage()
    {
        hits = Mathf.Clamp(hits-1, 0, maxHits);
        
        if (hits <= 0) 
            Break();
    }

    private void Break()
    {
        breakEvent.Invoke();
    }
}