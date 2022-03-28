using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public string stateName;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.stateName = name;
        this.stateMachine = stateMachine;
    }

    public virtual void DoDelayAction() { }

    public virtual void Enter()
    {
        Debug.Log(stateName);
    }

    public virtual void UpdateLogic() { }

    public virtual void UpdatePhysics() { }

    public virtual void Exit() { }
}

public class BossIdle : BaseState
{
    public BossIdle(BossBehaviour stateMachine) : base("Idle", stateMachine)
    {
        _sm = stateMachine;
    }

    private BossBehaviour _sm;

    public override void Enter()
    {
        base.Enter();
        _sm.animator.SetBool("isIdle", true);
        _sm.StartDelayAction(this, _sm.timeIdle);
        
    }

    public override void DoDelayAction()
    {
        base.DoDelayAction();
        _sm.ChangeState(_sm.shootingSecondPatternState);
    }
}

public class BossDisappear : BaseState
{
    private BossBehaviour _sm;

    float fadeRate = 0.5f;
    Color tmpColor;

    public BossDisappear(BossBehaviour stateMachine) : base(nameof(BossDisappear), stateMachine)
    {
        _sm = stateMachine;
        fadeRate = _sm.fadeRate;
    }

    public override void Enter()
    {
        base.Enter();
        tmpColor = _sm.SR.color;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Disappear();
    }

    void Disappear()
    {
        tmpColor.a -= Time.deltaTime * fadeRate;
        _sm.SR.color = tmpColor;
        if (tmpColor.a <= 0)
        {
            _sm.ChangeState(_sm.invisibleState);
        }
    }
}

public class BossInvisible : BaseState
{
    private BossBehaviour _sm;
    float timeInvisible = 3f;
    float startTimeInvisible;

    public BossInvisible(BossBehaviour stateMachine) : base("BossInvisible", stateMachine)
    {
        _sm = stateMachine;
        timeInvisible = _sm.timeInvisible;
    }

    public override void Enter()
    {
        base.Enter();
        foreach (Collider2D col in _sm.colliders)
        {
            col.enabled = false;
        }
        startTimeInvisible = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Invisible();
    }

    void Invisible()
    {
        if (startTimeInvisible < timeInvisible)
        {
            startTimeInvisible += Time.deltaTime;
        }
        else
        {
            _sm.ChangeState(_sm.reappearState);
        }
    }
}

public class BossReappear : BaseState
{
    BossBehaviour _sm;

    float fadeRate = 0.5f;
    Color tmpColor;
    //fixed positions to appear in the room
    Vector2[] newPos = {new Vector2(11f,5.5f), new Vector2(11f, -5.5f), new Vector2(-11f, 5.5f), new Vector2(-11f, -5.5f) };
    Vector2 appearPos;

    public BossReappear(BossBehaviour stateMachine) : base(nameof(BossReappear), stateMachine)
    {
        _sm = stateMachine;
        fadeRate = _sm.fadeRate;
    }

    public override void Enter()
    {
        base.Enter();
        tmpColor = _sm.SR.color;
        foreach (Collider2D col in _sm.colliders)
        {
            col.enabled = true;
        }

        appearPos = _sm.rootPos + newPos[Random.Range(0, 4)];
        _sm.transform.position = appearPos;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Reappear();
    }

    void Reappear()
    {
        tmpColor.a += Time.deltaTime * fadeRate;
        _sm.SR.color = tmpColor;
        if (tmpColor.a >= 1)
        {
            _sm.ChangeState(_sm.shootingFirstPatternState);
        }
    }
}

public class BossShootingFirstPattern : BaseState
{
    BossBehaviour _sm;

    Transform bullet;
    Vector3 spreadAngle;
    Vector3 spreadOffset;
    int timePattern; // how many patterns have shoot

    public delegate void OnDelayAction();
    public OnDelayAction ActionDelayed;

    public BossShootingFirstPattern(BossBehaviour stateMachine) : base(nameof(BossShootingFirstPattern), stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        timePattern = 0;
        ActionDelayed = ShootPattern;
        ShootPattern();
    }

    void ShootPattern()
    {
        for (int i = 0; i < _sm.numberOfBulletFirstPattern; i++)
        {
            spreadAngle = new Vector3(0f, 0f, 360f / _sm.numberOfBulletFirstPattern);
            spreadOffset = new Vector3(0f, 0f, 180f / _sm.numberOfBulletFirstPattern);
            bullet = BulletsPool.instance.GetPooledGameObject(_sm.bulletTypeFirstPattern).transform;
            if (bullet != null)
            {
                bullet.GetComponent<BulletCtrlr>().SetUp(_sm.bulletDamageFP, _sm.bulletSpeedFP);
                bullet.gameObject.SetActive(true);
                bullet.position = _sm.myTransform.position;
                bullet.Rotate(spreadAngle * i + spreadOffset * timePattern);
            }
        }

       if(timePattern < _sm.numberOfTimeShootFirstPattern - 1) // -1 for initial
        {
            _sm.StartDelayAction(this, _sm.delayTimeBtwTimesFirstPattern);
            timePattern++;
        }
        else
        {
            ActionDelayed = ChangeToAnotherState;
            _sm.StartDelayAction(this, 3f);
        }
    }

    void ChangeToAnotherState()
    {
        if(_sm.numberOfFlashes <= 0)
        {
            _sm.ChangeState(_sm.movingState);
            _sm.numberOfFlashes = _sm.startNumberOfFlashes;
        }
        else
        {
            _sm.ChangeState(_sm.disappearState);
            _sm.numberOfFlashes--;
        }
    }

    public override void DoDelayAction()
    {
        base.DoDelayAction();
        ActionDelayed();
    }
}

public class BossShootingSecondPattern : BaseState
{
    BossBehaviour _sm;

    Transform bullet;
    Vector3 spreadAngle;
    Transform playerTf;

    int timePattern; // how many patterns have shoot

    public delegate void OnDelayAction();
    public OnDelayAction ActionDelayed;

    public BossShootingSecondPattern(BossBehaviour stateMachine) : base(nameof(BossShootingSecondPattern), stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        timePattern = 0;
        ActionDelayed = ShootPattern;
        ShootPattern();
    }

    void ShootPattern()
    {
        playerTf = PlayerMovement.instance.GetTransform();

        for (int i = 0; i < _sm.numberOfBulletSecondPattern; i++)
        {
            if (i % 2 == 0 && i != 0)
            {
                spreadAngle = new Vector3(0, 0, (i - 1) * 5);
            }
            else if (i % 2 == 1)
            {
                spreadAngle = new Vector3(0, 0, i * -5);
            }
            else
            {
                spreadAngle = Vector3.zero;
            }

            bullet = BulletsPool.instance.GetPooledGameObject(_sm.bulletTypeFirstPattern).transform;
            if (bullet != null)
            {
                bullet.GetComponent<BulletCtrlr>().SetUp(_sm.bulletDamageFP, _sm.bulletSpeedFP);
                bullet.gameObject.SetActive(true);
                bullet.position = _sm.myTransform.position;
                bullet.up = new Vector2(playerTf.position.x - bullet.position.x, playerTf.position.y - bullet.position.y);
                bullet.Rotate(spreadAngle);
            }
        }


        if (timePattern < _sm.numberOfTimeShootSecondPattern - 1) // -1 for initial
        {
            _sm.StartDelayAction(this, _sm.delayTimeBtwTimesSecondPattern);
            timePattern++;
        }
        else
        {
            ActionDelayed = ChangeToAnotherState;
            _sm.StartDelayAction(this, 3f);
        }
    }

    void ChangeToAnotherState()
    {
        _sm.ChangeState(_sm.disappearState);
    }

    public override void DoDelayAction()
    {
        base.DoDelayAction();
        ActionDelayed();
    }

}

public class BossMoving : BaseState
{
    BossBehaviour _sm;
    Vector2 destination;

    public BossMoving(BossBehaviour stateMachine) : base(nameof(BossMoving), stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.particleGO.SetActive(true);
        _sm.particleGO.GetComponent<ParticleSystem>().Play();
        destination = PlayerMovement.instance.GetTransform().position;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _sm.canDealDamage = true;
        _sm.gameObject.layer = 12; // change to PlayerDamage
        _sm.myTransform.position = Vector2.MoveTowards(_sm.myTransform.position,  destination , _sm.speed * Time.deltaTime);
        if(_sm.myTransform.position == (Vector3)destination)
        {
            _sm.canDealDamage = false;
            _sm.gameObject.layer = 6;
            _sm.ChangeState(_sm.shootingSecondPatternState);
        }
    }

    

    public override void Exit()
    {
        base.Exit();
        _sm.StartDelayAction(this, 2f);
    }

    public override void DoDelayAction()
    {
        base.DoDelayAction();
        _sm.particleGO.SetActive(false);

    }
}