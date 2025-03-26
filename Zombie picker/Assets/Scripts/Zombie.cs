using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed = 10f;
    public float health = 10;
    public float damage = 5;
    public float attackDistance = 1.5f;
    public float attackRate = 1f;
    public float detectRange = 5f;

    private Transform target;
    private bool isAttacking = false;
    private Rigidbody rb;
    private Vector3 forwardDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        forwardDirection = transform.forward; // Изначально движемся вперед
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

    private IEnumerator Attack()
    {
        isAttacking = true;

        while (target != null && Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
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
