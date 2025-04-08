using System.Collections;
using UnityEngine;

public class ZombieBoss : Zombie
{
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        
        var clonesInRange = Physics.OverlapSphere(transform.position, 1000);
            
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackRate);

        foreach (var hit in clonesInRange)
        {
            if (hit == null || !hit.CompareTag("Player")) continue;
            
            var clone = hit.GetComponent<Clone>();
            clone.TakeDamage(damage); 
        }

        target = null;
        
        isAttacking = false;
    }
}