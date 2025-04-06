using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletController))]
public class Clone : MonoBehaviour
{
    [NonSerialized] public float CurrentHealth; 
    
    [Header("Health")]
    public float maxHealth = 100;
    
    [Header("Movement Settings")]
    public float followSpeed = 5f;
    public float separationDistance = 1f;
    public float separationWeight = 2f;
    
    [Header("Gun")]
    public GunData currentGun;
    
    private Rigidbody rb;
    private BulletController bulletController;
    
    public event Action OnHealthChanged;
    public event Action OnHealTriggered;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletController = GetComponent<BulletController>();
        
        if (currentGun.fireRate == 0) return;
        
        InvokeRepeating(nameof(Shoot), 0f, 1 / currentGun.fireRate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstAidKit"))
        {
            OnHealTriggered?.Invoke();
            Destroy(other.gameObject);
        }
    }

    public void Follow(Vector3 targetPosition, List<Clone> allClones)
    {
        var cohesionDirection = (targetPosition - transform.position).normalized;

        var separationForce = Vector3.zero;
        var neighbors = 0;
        foreach (var clone in allClones)
        {
            if (clone == this)
                continue;

            var offset = transform.position - clone.transform.position;
            var distance = offset.magnitude;
            
            if (!(distance < separationDistance) || !(distance > 0)) continue;

            separationForce += offset.normalized / distance;
            neighbors++;
        }
        if (neighbors > 0)
            separationForce /= neighbors;

        var finalDirection = (cohesionDirection + separationForce * separationWeight).normalized;
        
        var newPosition = transform.position + finalDirection * (followSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }
    
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        OnHealthChanged?.Invoke();
        if (CurrentHealth <= 0) 
            Die();
    }

    public void Heal()
    {
        CurrentHealth = 100;
        OnHealthChanged?.Invoke();
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
    
    private void Shoot()
    {
        bulletController.Shoot(currentGun, transform.position);
    }

    public void ChangeWeapon(GunData newGun)
    {
        currentGun = newGun;
    }
}