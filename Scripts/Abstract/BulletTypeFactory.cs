using UnityEngine;

public abstract class BulletCtrlr : MonoBehaviour
{
    protected bool isForced;
    protected int damage;
    protected float speed;
    protected bool hasTrail;
    protected float bulletWidth;
    protected float bulletLength;
    protected int bounceCount;

    protected Transform myTransform;
    protected Rigidbody2D rb2d;
    protected Bullet bullet;

    protected virtual void Awake()
    {
        bullet = GetComponent<Bullet>();
        myTransform = transform;

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2d))
        {
            rb2d = rigidbody2d;
        }
    }

    public void SetUp(int _damage, float _speed, bool _hasTrail, float _bulletLength, float _bulletWidth, int _bounceCount)
    {
        SetUp(_damage, _speed);
        hasTrail = _hasTrail;
        bulletLength = _bulletLength;
        bulletWidth = _bulletWidth;
        bounceCount = _bounceCount;
        AdditionalSetUp();
    }

    public void SetUp(int _damage, float _speed)
    {
        damage = _damage;
        speed = _speed;
    }

    public abstract void AdditionalSetUp();

    public void OnTriggerEnter2D()
    {
        isForced = false;
    }
}
