using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletTypes bulletType;
    public BulletFlyType bulletFlyType;
    [SerializeField] int damage;
    [SerializeField] float speed;

    private void Awake()
    {
        switch (bulletType)
        {
            case BulletTypes.EnemyBullet:
                gameObject.AddComponent<EnemyBulletController>();
                break;
            case BulletTypes.Arrow:
            case BulletTypes.IceArrow:
                gameObject.AddComponent<ArrowCtrlr>();
                break;
            case BulletTypes.Spell:
                gameObject.AddComponent<SpellCtrlr>();
                break;
        }
    }
}


public enum BulletTypes
{
    EnemyBullet,
    Arrow,
    IceArrow,
    Spell,
}

public enum BulletFlyType
{
    straight
}