using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : StateMachine
{
    [HideInInspector] public BossIdle idleState;
    [HideInInspector] public BossDisappear disappearState;
    [HideInInspector] public BossInvisible invisibleState;
    [HideInInspector] public BossReappear reappearState;
    [HideInInspector] public BossMoving movingState;
    [HideInInspector] public BossShootingFirstPattern shootingFirstPatternState;
    [HideInInspector] public BossShootingSecondPattern shootingSecondPatternState;

    public Rigidbody2D RB2D;
    public Transform myTransform;
    public Animator animator;
    public SpriteRenderer SR;
    public Collider2D[] colliders;
    public float speed;
    public Vector2 rootPos;
    public GameObject particleGO;
    public float fadeRate;

    public float timeIdle;
    public float timeInvisible;

    public bool canDealDamage;
    public bool canDealDamageAgain = true;

    public int damage;

    [HideInInspector] public int numberOfFlashes;
    public int startNumberOfFlashes; // fixed one

    [Header("FIRST PATTERN")]
    public int bulletDamageFP;
    public float bulletSpeedFP;
    public int numberOfBulletFirstPattern;
    public BulletTypes bulletTypeFirstPattern;
    public int numberOfTimeShootFirstPattern;
    public float delayTimeBtwTimesFirstPattern;

    [Header("SECOND PATTERN")]
    public int bulletDamageSP;
    public float bulletSpeedSP;
    public int numberOfBulletSecondPattern;
    public BulletTypes bulletTypeSecondPattern;
    public int numberOfTimeShootSecondPattern;
    public float delayTimeBtwTimesSecondPattern;

    private void Awake()
    {
        idleState = new BossIdle(this);
        disappearState = new BossDisappear(this);
        invisibleState = new BossInvisible(this);
        reappearState = new BossReappear(this);
        movingState = new BossMoving(this);
        shootingFirstPatternState = new BossShootingFirstPattern(this);
        shootingSecondPatternState = new BossShootingSecondPattern(this);

        numberOfFlashes = startNumberOfFlashes;
        canDealDamageAgain = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canDealDamage && collision.gameObject.layer == 8 && canDealDamageAgain) // collide player
        {
            StartCoroutine(DelayDealDamage());
            collision.gameObject.GetComponent<PlayerStatController>().GetHealthSystem().TakeDamage(damage);
        }
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }

    IEnumerator CallWithDelay(BaseState _state, float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        _state.DoDelayAction();
    }

    public void StartDelayAction(BaseState _state, float _delayTime)
    {
        StartCoroutine(CallWithDelay(_state, _delayTime));
    }

    IEnumerator DelayDealDamage()
    {
        canDealDamageAgain = false;
        yield return new WaitForSeconds(1f);
        canDealDamageAgain = true;
    }
}
