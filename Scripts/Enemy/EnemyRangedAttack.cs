using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : Abs_RangedAttack
{
    [SerializeField] EnemyData enemyData;
    Transform playerTf;
    bool canContinueClick = true;
    Transform bullet;
    List<Transform> bullets = new List<Transform>();

    private void Awake()
    {
        canContinueClick = true;
        enemyData = GetComponent<EnemyBehaviour>().GetEnemyData();
        startTimeBtwAttacks = enemyData.timeBtwAttacks;
        timeBtwAttacks = startTimeBtwAttacks;
        bulletType = enemyData.bulletType;
        bulletPos = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CanAttack)
        {
            StartCoroutine(DoRangedAttack());
        }
    }

    public override IEnumerator DoRangedAttack()
    {
        if (timeBtwAttacks < 0 && canContinueClick)
        {
            canContinueClick = false;
            for (int i = 0; i < NumberOfBulletsPattern; i++)
            {
                wpAnimator.SetTrigger("isAttacking");
                CreateBullet();
                yield return new WaitForSeconds(0.1f);
            }
            canContinueClick = true;
            timeBtwAttacks = startTimeBtwAttacks;
        }
        else
        {
            timeBtwAttacks -= Time.deltaTime;
        }
    }

    public override List<Transform> CreateBullet()
    {
        bullet = BulletsPool.instance.GetPooledGameObject(bulletType).transform;
        if(bullet != null)
        {
            bullet.GetComponent<BulletCtrlr>().SetUp(enemyData.damage, enemyData.bulletSpeed);
            bullet.gameObject.SetActive(true);
            bullet.position = bulletPos.position;
            playerTf = PlayerMovement.instance.GetTransform();
            bullet.up = (playerTf.position - bullet.transform.position).normalized;
        }
        return bullets;
    }
}

