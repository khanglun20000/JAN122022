using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMeleeAttack : Abs_MeleeAttack
{
    private SwordData wpDataPrivate;

    public SwordData WpData
    {
        get { return wpDataPrivate; }
        set
        {
            wpDataPrivate = value;
            UpdateWpData();
        }
    }
    public UnityEvent hitEvent;

    Collider2D[] enemyToDamage;
    Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        hitEvent.AddListener(Hit);
        myTransform = transform;
        attackPos.gameObject.SetActive(true);
        CanAttack = true;
        UpdateWpData();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CanAttack)
        {
            DoMeleeAttack();
        }
    }

    public override void DoMeleeAttack()
    {
        if (timeBtwAttacks < 0)
        {
            if (Input.GetMouseButton(0))
            {
                wpAnimator.SetTrigger("isAttacking");
                timeBtwAttacks = startTimeBtwAttacks;
            }
        }
        else
        {
            timeBtwAttacks -= Time.deltaTime;
        }
    }

    void Hit()
    {
        enemyToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemy);
        for (int i = 0; i < enemyToDamage.Length; i++)
        {
            if(enemyToDamage[i].gameObject.layer == 6) //collide enemy
            {
                Debug.Log(enemyToDamage[i].name);
                Vector2 difference = (enemyToDamage[i].transform.position - myTransform.position).normalized;
                enemyToDamage[i].transform.GetComponent<EnemyBehaviour>().GetGotShot(difference, 50, 0.3f, weaponDamage, null);
            }
            else if(enemyToDamage[i].gameObject.layer == 10) //collide enemy bullet
            {
                BulletsPool.instance.ReturnToPool(enemyToDamage[i].gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange); 
    }

    public override void UpdateWpData()
    {
        weaponDamage = wpDataPrivate.damage;
        attackRange = wpDataPrivate.range;
        startTimeBtwAttacks = wpDataPrivate.attackSpeed;
    }
}
