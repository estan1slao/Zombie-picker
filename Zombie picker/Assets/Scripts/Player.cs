using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth; 
    
    [Header("Movement Settings")]
    public float followSpeed = 5f;
    public float separationDistance = 1f;
    public float separationWeight = 2f;
    
    private Rigidbody rb;
    
    public event Action OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void Follow(Vector3 targetPosition, List<Player> allClones)
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
        currentHealth -= damage;
        OnHealthChanged?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}