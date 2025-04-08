using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [NonSerialized] public GunData GunData;
    private Rigidbody rb;
    private Coroutine moveRoutine;
    
    private void Awake() => rb = GetComponent<Rigidbody>();

    private void Start() => moveRoutine = StartCoroutine(Move());

    private IEnumerator Move()
    {
        var lifeTime = GunData.lifeTime;
        
        while (lifeTime > 0)
        {
            rb.MovePosition(rb.position + Vector3.left * (GunData.speed * Time.deltaTime));
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Zombie>().TakeDamage(GunData.damage);
            Destroy(gameObject);   
        }
        else if (other.CompareTag("Barrel"))
        {
            other.GetComponent<Barrel>().TakeDamage();
            Destroy(gameObject);   
        }
        else if (other.CompareTag("Wall"))
        {
            other.GetComponent<WallController>().TakeDamage();
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy() => StopCoroutine(moveRoutine);
}