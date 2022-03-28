using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerRangedAttack : Abs_RangedAttack
{
    [SerializeField] Transform wpHolderTransform;
    private RangedWeaponData wpDataPrivate;

    public RangedWeaponData WpData
    {
        get { return wpDataPrivate; }
        set
        {
            wpDataPrivate = value;
            UpdateWpData();
        }
    }

    public delegate List<Transform> OnCreateBullet();
    //called in WeaponCtrlr
    public OnCreateBullet BulletCreated;

    List<Transform> bullets = new List<Transform>();
    Transform bullet;

    Vector3 spreadAngle;
    [SerializeField] float spreadOffset = 15f;
    [SerializeField] float timeOffsetPattern = 0.2f;
    bool canContinueClick = true; // avoid multiple getmousebutton calls in one click


    // Start is called before the first frame update
    void Start()
    {
        canContinueClick = true;
        BulletCreated += CreateBullets;
        bulletPos.gameObject.SetActive(true);
        UpdateWpData();

    }

    void FixedUpdate()
    {
        DoAttack();
    }

    public override IEnumerator DoRangedAttack()
    {
        if (timeBtwAttacks < 0)
        {
            if (Input.GetMouseButton(0) && canContinueClick)
            {
                canContinueClick = false;
                bullets.Clear();
                for (int i = 0; i < NumberOfBulletsPattern; i++)
                {
                    wpAnimator.SetTrigger("isAttacking");
                    yield return new WaitForSeconds(timeOffsetPattern);
                }
                canContinueClick = true;
                timeBtwAttacks = startTimeBtwAttacks;
            }
        }
        else
        {
            timeBtwAttacks -= Time.deltaTime;
        }
    }

    private List<Transform> CreateBullets()
    {
        foreach( Transform bullet in CreateBullet())
            bullet.GetComponent<PlayerBulletController>().SetUp(weaponDamage, WpData.bulletSpeed, WpData.hasTrail, WpData.bulletLength, WpData.bulletWidth, bounceCount);
        return bullets;
    }

    public override List<Transform> CreateBullet()
    {
        for(int i = 0; i < BulletsEachPattern; i++)
        {
            GetSpreadAngle(i);
            bullet = BulletsPool.instance.GetPooledGameObject(bulletType).transform;
            if (bullets != null)
            {
                bullet.SetPositionAndRotation(bulletPos.position, bulletPos.rotation);
                bullet.gameObject.SetActive(true);
                bullet.Rotate(spreadAngle);
            }
            bullet.SetParent(wpHolderTransform);
            bullets.Add(bullet);
        }
        return bullets;
    }

    private void GetSpreadAngle(int _bulletIndex)
    {
        if (BulletsEachPattern % 2 == 1)
        {
            if (_bulletIndex == 0)
            {
                spreadAngle = Vector3.zero;
            }
            else if (_bulletIndex % 2 == 0)
            {
                spreadAngle = new Vector3(0f, 0f, spreadOffset * _bulletIndex);
            }
            else if (_bulletIndex % 2 == 1)
            {
                spreadAngle = new Vector3(0f, 0f, -spreadOffset * (_bulletIndex + 1));
            }
        }
        else if (BulletsEachPattern % 2 == 0)
        {
            if (_bulletIndex % 2 == 0)
            {
                spreadAngle = new Vector3(0f, 0f, -spreadOffset * (_bulletIndex + 1));
            }
            else if (_bulletIndex % 2 == 1)
            {
                spreadAngle = new Vector3(0f, 0f, spreadOffset * _bulletIndex);
            }
        }
    }

    public override void UpdateWpData()
    {
        bulletType = wpDataPrivate.bulletType;
        startTimeBtwAttacks = wpDataPrivate.attackSpeed;
        weaponDamage = wpDataPrivate.damage;
    }
}
