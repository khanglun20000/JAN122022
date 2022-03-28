using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName ="New Enemy")]
public class EnemyData : ScriptableObject
{
    public EnemyCombatStyle enemyCombatStyle;

    //movement variables
    public Vector2 movementSpeedRange;
    public int chaseSpeed;
    public float detectRange;
    public float attackRange;
    public float attackRangeOffset;

    //stat variables
    public int maxHealth;
    public int killExp;

    //attack variables
    public float timeBtwAttacks;
    public int damage;

    //ranged variables
    public BulletTypes bulletType;
    public float bulletSpeed;
}

public enum EnemyCombatStyle{
    Melee,
    Ranged
}
