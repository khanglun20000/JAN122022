using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrlr : MonoBehaviour
{
    [SerializeField] PlayerRangedAttack PRA;
    [SerializeField] PlayerMeleeAttack PMA;

    List<Transform> newBullets;

    void CreateBullet()
    {
        newBullets = PRA.BulletCreated();
    }

    //called in animation of bow
    void ShootBullet()
    {
        foreach(Transform newBullet in newBullets)
        {
            PlayerBulletController bulletCtrlr = newBullet.GetComponent<PlayerBulletController>();
            bulletCtrlr.bulletShot();
        }
        newBullets.Clear();
    }

    //called in animation of sword
    void HitEnemy()
    {
        PMA.hitEvent.Invoke();
    }
}
