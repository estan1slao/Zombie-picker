using System;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public GameObject particle;
    
    private void Update()
    {
        if (transform.position.x >= 20) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Zombie>().TakeDamage(200);
            Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
