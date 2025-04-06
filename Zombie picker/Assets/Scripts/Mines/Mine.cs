using System;
using UnityEngine;

public class Mine : MonoBehaviour
{
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
            Destroy(gameObject);
        }
    }
}
