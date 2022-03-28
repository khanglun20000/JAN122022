using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : Abs_MeleeAttack
{
    EnemyData enemyData;

    private void Awake()
    {
        enemyData = GetComponent<EnemyBehaviour>().GetEnemyData();
        startTimeBtwAttacks = enemyData.timeBtwAttacks;
        weaponDamage = enemyData.damage;
        attackRange = enemyData.attackRange;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoMeleeAttack();
    }

    public override void DoMeleeAttack()
    {
        if (timeBtwAttacks < 0 && CanAttack)
        {
            wpAnimator.SetTrigger("isAttacking");
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, whatIsEnemy);

            if(hit != null)
            {
                hit.GetComponent<PlayerStatController>().GetHealthSystem().TakeDamage(weaponDamage);
            }

            timeBtwAttacks = startTimeBtwAttacks;
        }
        else
        {
            timeBtwAttacks -= Time.deltaTime;
        }
    }
}
