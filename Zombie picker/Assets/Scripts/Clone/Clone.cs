using System;
using System.Collections;
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
    
    [Header("Gun Visuals")]
    public Transform weaponSlot;
    private GameObject currentGunVisual;

    public GunData pistol;
    
    public event Action OnHealthChanged;
    public event Action OnHealTriggered;
    public event Action<GunData> OnGunChangeTriggered;

    private Coroutine shootRoutine;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletController = GetComponent<BulletController>();

        ChangeWeapon(pistol);
        
        if (currentGun.fireRate == 0) return;

        shootRoutine = StartCoroutine(Shoot());
    }

    private void OnDestroy() => StopCoroutine(shootRoutine);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstAidKit"))
        {
            OnHealTriggered?.Invoke();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Gun"))
        {
            OnGunChangeTriggered?.Invoke(other.GetComponent<Gun>().GunData);
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
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
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
    
    private IEnumerator Shoot()
    {
        while (true)
        {       
            yield return new WaitForSeconds(1 / currentGun.fireRate);
            bulletController.Shoot(currentGun, weaponSlot.position);
        }
    }

    public void ChangeWeapon(GunData newGun)
    {
        currentGun = newGun;

        // Удаляем старое оружие, если было
        if (currentGunVisual != null)
            Destroy(currentGunVisual);

        // Спавним новое визуальное оружие
        if (currentGun.gunPrefab != null && weaponSlot != null)
        {
            currentGunVisual = Instantiate(currentGun.gunPrefab, weaponSlot);

            // Задаём локальную позицию и поворот из GunData
            currentGunVisual.transform.localPosition = currentGun.localPosition;
            currentGunVisual.transform.localEulerAngles = currentGun.localRotation;
            currentGunVisual.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

}