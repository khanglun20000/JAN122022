using System.Collections;
using UnityEngine;

public class EnemyBulletController : BulletCtrlr
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        isForced = true;
    }


    protected void FixedUpdate()
    {
        if (isForced)
        {
            ShootBullet();
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D();

        if (collision.gameObject.layer == 8) // collide player
        {
            collision.GetComponent<PlayerStatController>().GetHealthSystem().TakeDamage(damage);
        }
        BulletsPool.instance.ReturnToPool(gameObject);
    }

    public void ShootBullet()
    {
        rb2d.velocity = speed * 100 * Time.deltaTime * transform.up;
    }


    private void OnDisable()
    {
        myTransform.position = Vector3.zero;
        myTransform.rotation = Quaternion.identity;
    }

    public override void AdditionalSetUp(){    }
}

public abstract class PlayerBulletController : BulletCtrlr
{
    protected abstract void BulletFly();

    float knockTime = 0.4f;

    protected bool canCollide = true;
    protected bool canBounce = true;

    public delegate void OnShootBullet();
    public OnShootBullet bulletShot;

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable()
    {
        if (hasTrail)
        {
            myTransform.GetChild(0).GetComponent<TrailRenderer>().Clear();
        }
    }

    private void FixedUpdate()
    {
        if (isForced)
        {
            BulletFly();
        }
    }

    private void LateUpdate()
    {
        canCollide = true;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (canCollide)
        {
            canCollide = false;
            if (collision2D.gameObject.layer == 6) //hit enemy 
            {
                Vector2 difference = (collision2D.transform.position - transform.position).normalized;
                collision2D.transform.GetComponent<EnemyBehaviour>().GetGotShot(difference, speed, knockTime, damage, myTransform);
                ActionsOnHitEnemy();
            }
            else if(bounceCount <= 0)
            {
                ActionsOnStopBounce();
            }
            else if(bounceCount > 0)
            {
                transform.up = Vector3.Reflect(myTransform.up, collision2D.contacts[0].normal);
                bounceCount--;
                ActionsOnHit();
            }
        }
    }

    protected virtual void ShootBullet()
    {
        isForced = true;
    }

    protected virtual void ActionsOnStopBounce()
    {

    }

    protected virtual void ActionsOnHit()
    {

    }

    protected virtual void ActionsOnHitEnemy()
    {

    }
}


public class ArrowCtrlr : PlayerBulletController
{
    bool canFade = true;
    SpriteRenderer SR;
    Color spriteColor;
    Color tmpColor;

    protected override void BulletFly()
    {
        rb2d.velocity = speed * 100 * Time.deltaTime * transform.up;
    }

    public override void AdditionalSetUp()
    {
        SR.color = spriteColor;
    }

    protected override void Awake()
    {
        base.Awake();
        SR = myTransform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteColor = SR.color;
        tmpColor = SR.color;
        bulletShot = ShootBullet;
        tmpColor *= (float)Random.Range(45f, 70f) / 100f;
        tmpColor.a = 1;
    }

    protected override void OnCollisionEnter2D(Collision2D collision2D)
    {
        base.OnCollisionEnter2D(collision2D);
    }

    protected override void ActionsOnHitEnemy()
    {
        base.ActionsOnHitEnemy();
        Stuck();
    }

    protected override void ActionsOnStopBounce()
    {
        base.ActionsOnStopBounce();
        Stuck();
        StartCoroutine(InvokeDisable());
        if (canFade)
        {
            Invoke(nameof(Fade), 2f);
        }
    }

    protected override void ShootBullet()
    {
        base.ShootBullet();
        rb2d.isKinematic = false;
        GetComponent<Collider2D>().enabled = true;
        myTransform.SetParent(null);
    }

    IEnumerator InvokeDisable()
    {
        yield return new WaitForSeconds(10f);
        BulletsPool.instance.ReturnToPool(gameObject);
    }

    void Stuck()
    {
        GetComponent<Collider2D>().enabled = false;
        isForced = false;
        rb2d.isKinematic = true;
        rb2d.velocity = Vector2.zero;
        SR.sortingOrder = -10;
    }

    private void Fade()
    {
        SR.color = tmpColor;
    }
}

public class SpellCtrlr : PlayerBulletController
{
    [SerializeField] float maxSpeed;
    [SerializeField] float currentSpeed;
    [SerializeField] float time = 0;

    protected override void Awake()
    {
        base.Awake();
        bulletShot = ShootBullet;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ShootBullet();
    }

    protected override void BulletFly()
    {
        ChangeSpeedGradually();
    }

    public override void AdditionalSetUp()
    {
        maxSpeed = speed;
        currentSpeed = speed;
        myTransform.SetParent(null);
        time = 0;
    }

    protected override void ActionsOnStopBounce()
    {
        base.ActionsOnStopBounce();
        isForced = false;
        BulletsPool.instance.ReturnToPool(gameObject);
    }

    void ChangeSpeedGradually()
    {
        if (currentSpeed <= maxSpeed)
        {
            currentSpeed = speed * (1 - Mathf.Cos((time - 0.15f) * Mathf.PI / 2));
        }
        else
        {
            currentSpeed = maxSpeed;
        }
        time += Time.deltaTime;
        rb2d.velocity = currentSpeed * 100 * Time.deltaTime * transform.up;
    }
}