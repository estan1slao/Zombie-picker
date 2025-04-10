using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    private BulletController bulletController;
    
    [Header("Gun Visuals")]
    public Transform weaponSlot;
    private GameObject currentGunVisual;

    public GunData pistol;

    [Header("Other")] 
    public GameObject deadPrefab;
    
    public event Action OnHealthChanged;
    public event Action OnHealTriggered;
    public event Action<GunData> OnGunChangeTriggered;

    private Coroutine shootRoutine;
    
    private Vector3 lastPosition;
    private Vector3 smoothedDirection = Vector3.zero;
    public float directionSmoothTime = 0.1f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
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
        targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0;

        float moveThresholdSqr = 0.05f * 0.05f;
        if (toTarget.sqrMagnitude < moveThresholdSqr)
            return;

        var cohesionDirection = toTarget.normalized;

        var separationForce = Vector3.zero;
        int neighbors = 0;

        for (int i = 0; i < allClones.Count; i++)
        {
            var other = allClones[i];
            if (other == this) continue;

            var offset = transform.position - other.transform.position;
            float distanceSqr = offset.sqrMagnitude;

            if (distanceSqr < separationDistance * separationDistance && distanceSqr > 0.0001f)
            {
                separationForce += offset / distanceSqr;
                neighbors++;
            }
        }

        if (neighbors > 0)
            separationForce /= neighbors;

        var targetDirection = (cohesionDirection + separationForce * separationWeight).normalized;

        smoothedDirection = Vector3.Lerp(smoothedDirection, targetDirection, directionSmoothTime);

        transform.Translate(smoothedDirection * followSpeed * Time.deltaTime, Space.World);
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
        Instantiate(deadPrefab, transform.position, Quaternion.Euler(0, -90, 0));
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

            currentGunVisual.GetComponent<Outline>().enabled = false;
        }
    }

}