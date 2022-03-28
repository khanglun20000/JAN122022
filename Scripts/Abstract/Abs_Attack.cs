using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Abs_Attack : MonoBehaviour
{
    public bool CanAttack;
    public float timeBtwAttacks;
    [SerializeField] protected int weaponDamage;
    public int WeaponDamage
    {
        get
        {
            return weaponDamage;
        }
        set
        {
            weaponDamage = value;
        }
    }

    [SerializeField] protected float startTimeBtwAttacks;
    [SerializeField] protected Animator wpAnimator;

    public abstract void DoAttack();
    public virtual void UpdateWpData() { }
}

public abstract class Abs_RangedAttack : Abs_Attack
{
    [SerializeField] protected Transform bulletPos;
    [SerializeField] protected BulletTypes bulletType;

    public int NumberOfBulletsPattern = 1;
    public int BulletsEachPattern = 1;
    public int bounceCount = 0;

    public override void DoAttack()
    {
        StartCoroutine(DoRangedAttack());
    }
    public abstract IEnumerator DoRangedAttack();
    public abstract List<Transform> CreateBullet();
}


public abstract class Abs_MeleeAttack : Abs_Attack
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform attackPos;
    [SerializeField] protected float attackRange;

    public override void DoAttack()
    {
        DoMeleeAttack();
    }

    public abstract void DoMeleeAttack();
}