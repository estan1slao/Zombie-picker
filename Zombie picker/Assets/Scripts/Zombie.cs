using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public float speed = 10f;
    public float health = 10;
    public float damage = 5;
    public float attackDistance = 1.5f;
    public float attackRate = 1f;
    public float detectRange = 5f;
    
    public Slider healthBar;

    protected Transform target;
    protected bool isAttacking = false;
    private Rigidbody rb;
    private Vector3 forwardDirection;
    protected Animator animator;

    public AudioClip takeDamageSound;

    private AudioSource audioSource;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        forwardDirection = transform.forward; // Изначально движемся вперед
        healthBar.value = health;
        healthBar.maxValue = health;
        FindNearestTarget();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            FindNearestTarget();
        }

        if (!isAttacking)
        {
            if (target != null)
            {
                MoveTowardsTarget();
            }
            else
            {
                MoveForward(); // Если цели нет, идем прямо
            }
        }
    }
    
    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void MoveForward()
    {
        rb.MovePosition(rb.position + forwardDirection * speed * Time.fixedDeltaTime);
    }

    private void FindNearestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = detectRange; // Берем только игроков в пределах detectRange
        Transform closest = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player.transform;
            }
        }

        target = closest;
    }

    protected virtual IEnumerator Attack()
    {
        isAttacking = true;
        while (target != null && Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackRate);
            
            Clone clone = target.GetComponent<Clone>();
            if (clone != null)
            {
                clone.TakeDamage(damage);
            }

            yield return new WaitForSeconds(attackRate);

            if (target != null && target.GetComponent<Clone>().CurrentHealth <= 0)
            {
                target = null; // Сброс цели
                FindNearestTarget(); // Ищем нового игрока
            }
        }

        isAttacking = false;
    }

    public virtual void TakeDamage(float damage)
    {
        audioSource.PlayOneShot(takeDamageSound);
        health -= damage;
        StartCoroutine(SmoothFill(health));
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private protected IEnumerator SmoothFill(float targetValue)
    {
        var startValue = healthBar.value;
        var duration = 0.1f;
        var elapsed = 0f;
 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }
 
        healthBar.value = targetValue;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения игрока (синий)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Радиус атаки (красный)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
