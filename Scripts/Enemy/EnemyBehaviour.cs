using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform myTransform;
    [SerializeField] Rigidbody2D RB2D;
    [SerializeField] Vector2 movementSpeedRange;
    [SerializeField] Vector2 movementDistanceRange;
    [SerializeField] int chaseSpeed;
    [SerializeField] float detectRange;
    [SerializeField] float attackRange;
    //offset range that allow enemy return back to chasing state (to avoid enemy shaking when chasing player with lower speed than the enemy)
    [SerializeField] float attackRangeOffset;

    [SerializeField] Abs_Attack attackBehaviour;
    [SerializeField] EnemyData enemyData;

    [SerializeField] EnemyState enemyState;

    [SerializeField] bool canMove;

    [SerializeField] HealthSystem healthSystem;

    [SerializeField] bool canHurted = true;

    public List<Transform> stuckBullets = new List<Transform>();
    Transform playerTf;

    // variables for boucing off wall when got shot
    float knockBackForce;
    Vector2 knockBackDir;
    [SerializeField] bool canBounceWall = true;

    public enum EnemyState
    {
        Roaming,
        ChasingPlayer,
        AttackingPlayer,
        GotShot,
        Dead
    }
    public delegate void OnStateChanged(EnemyState firstState, EnemyState secondState);
    public OnStateChanged changeState;

    public delegate void OnGotShot(Vector2 dir, float force, float knockTime, int damage, Transform stuckBullet);
    public OnGotShot GetGotShot;

    int movementSpeed;
    // time offset for changing movement
    float changeMovementTime;
    // chance to move or idle
    int movementChance;
    // direction and distance to move
    Vector2 moveInput;

    private void Awake()
    {
        movementSpeedRange = enemyData.movementSpeedRange;
        chaseSpeed = enemyData.chaseSpeed;
        detectRange = enemyData.detectRange;
        attackRange = enemyData.attackRange;
        attackRangeOffset = enemyData.attackRangeOffset;

        healthSystem = new HealthSystem(enemyData.maxHealth);
        healthSystem.HealthOut += Die;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTf = PlayerMovement.instance.GetTransform();

        enemyState = EnemyState.Roaming;
        changeState += ChangeState;
        GetGotShot += GotShot;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Roaming:
                Roam();
                FindPlayer(detectRange, EnemyState.ChasingPlayer);
                break;
            case EnemyState.ChasingPlayer:
                Chase();
                FindPlayer(attackRange, EnemyState.AttackingPlayer);
                OutOfRange(detectRange, EnemyState.Roaming);
                break;
            case EnemyState.AttackingPlayer:
                Roam();
                OutOfRange(attackRange + attackRangeOffset, EnemyState.ChasingPlayer);
                break;
            case EnemyState.GotShot:
                break;
            case EnemyState.Dead:
                break;
        }
        
    }

    void Stop()
    {
        animator.SetBool("isMoving", false);
        moveInput = Vector2.zero;
    }

    void Roam()
    {
        // change to random direction every random period of time
        if (changeMovementTime <= 0)
        {
            movementChance = Random.Range(0, 101);
            if (movementChance <= 90)
            {
                animator.SetBool("isMoving", true);
                moveInput = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(movementDistanceRange.x, movementDistanceRange.y);
                changeMovementTime = Random.Range(0.6f, 0.9f);
            }
            else
            {
                Stop();
                changeMovementTime = Random.Range(1f, 1.5f);
            }
            movementSpeed = Random.Range((int)movementSpeedRange.x, (int)movementSpeedRange.y + 1);
        }
        else
        {
            changeMovementTime -= Time.deltaTime;
        }

        if (canMove)
        {
            RB2D.MovePosition(RB2D.position + (moveInput * Time.deltaTime * movementSpeed));
        }
    }

    void Chase()
    {
        if(canMove)
            myTransform.position = Vector2.MoveTowards(myTransform.position, playerTf.position, chaseSpeed * Time.deltaTime);
    }

    void FindPlayer(float _range, EnemyState _state)
    {
        if(Vector3.Distance(myTransform.position, playerTf.position) < _range && enemyState != _state)
        {
            changeState(enemyState, _state);
            enemyState = _state;
        }
    }

    void OutOfRange(float _range, EnemyState _state)
    {
        if (Vector3.Distance(myTransform.position, playerTf.position) > _range && enemyState != _state)
        {
            changeState(enemyState, _state);
            enemyState = _state;
        }
    }

    void ChangeState(EnemyState _firstState, EnemyState _secondState)
    {
        if(_firstState == EnemyState.ChasingPlayer && _secondState == EnemyState.AttackingPlayer)
        {
            attackBehaviour.CanAttack = true;
            //stop melee enemy while attacking
            if(enemyData.enemyCombatStyle == EnemyCombatStyle.Melee)
            {
                Stop();
            }
            
        }
        else if(_firstState == EnemyState.AttackingPlayer)
        {
            animator.SetBool("isMoving", true);
            attackBehaviour.CanAttack = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && enemyState != EnemyState.GotShot)
        {
            //reset changeMovementTime when collide with wall
            changeMovementTime = 0;
        }
        // bounce off wall 
        else if(collision.gameObject.CompareTag("Wall") && enemyState == EnemyState.GotShot && canBounceWall)
        {
            canBounceWall = false;
            RB2D.velocity = Vector2.zero;
            Vector2 NewDir = Vector2.Reflect(knockBackDir, collision.contacts[0].normal).normalized;
            RB2D.velocity  = knockBackForce * 50 * Time.deltaTime * NewDir;
        }
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    public void SetEnemyState(EnemyState _enemyState)
    {
        enemyState = _enemyState;
    }

    public EnemyState GetEnemyState()
    {
        return enemyState;
    }

    public void SetMoveBool(bool _bool)
    {
        canMove = _bool;
    }

    public void GotShot(Vector2 _dir, float _force, float _knockTime, int _damage, Transform _stuckBullet)
    {
        if (canHurted)
        {
            //set up for wall bouncing
            knockBackForce = _force;
            knockBackDir = _dir;
            canBounceWall = true;
            if (_stuckBullet)
            {
                stuckBullets.Add(_stuckBullet);
                _stuckBullet.SetParent(myTransform);
            }
            enemyState = EnemyState.GotShot;
            RB2D.velocity = _force * 100 * Time.deltaTime * _dir;
            StartCoroutine(KnockCo(_knockTime));
            healthSystem.TakeDamage(_damage);
        }
    }

    // return to roaming state after knocked back
    IEnumerator KnockCo(float _knockTime)
    {
        yield return new WaitForSeconds(_knockTime);
        RB2D.velocity = Vector2.zero;
        if(enemyState!= EnemyState.Dead)
        {
            enemyState = EnemyState.Roaming;
        }
    }

    void Die()
    {
        enemyState = EnemyState.Dead;
        GetComponent<Collider2D>().enabled = false;
        RB2D.velocity = Vector2.zero;
        canHurted = false;
        playerTf.GetComponent<PlayerStatController>().GetExpSystem().GainExp(enemyData.killExp);
        foreach(Transform stuckBullet in stuckBullets)
        {
            BulletsPool.instance.ReturnToPool(stuckBullet.gameObject);
        }
        animator.SetBool("isDead", true);
    }

    //called in Anim_EnemyDead
    void DestroyOnDead() 
    {
        Destroy(gameObject);
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(myTransform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(myTransform.position, attackRange);
    }
}
