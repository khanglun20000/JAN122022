using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrambleBallCtrlr : MonoBehaviour
{
    [SerializeField] Rigidbody2D RB2D;
    [SerializeField] float speed;
    [SerializeField] float accelAdd;
    [SerializeField] float maxSpeed;
    [SerializeField] Transform destroyPE;

    Animator animator;
    Vector2 dir;
    public UnityEvent StopEvent;

    bool canDealDamage;
    bool CanDealDamage
    {
        get { return canDealDamage; }
        set
        {
            RB2D.isKinematic = !value;
            canDealDamage = value;
        }
    }

    private void Start()
    {
        CanDealDamage = false;
        Invoke(nameof(StartMoving), 3f);
        animator = GetComponent<Animator>();
        StopEvent.AddListener(DestroyAction);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dir = Vector2.Reflect(RB2D.velocity, collision.contacts[0].normal).normalized;
        speed += accelAdd;
        RB2D.velocity = dir * Mathf.Max(speed, maxSpeed);

        if (collision.gameObject.CompareTag("Player") && canDealDamage)
        {
            PlayerStatController PSC = collision.gameObject.GetComponent<PlayerStatController>();
            PSC.GetHealthSystem().TakeDamage(Mathf.RoundToInt(PSC.GetHealthSystem().MaxHealth / 5));
        }
    }

    void StartMoving()
    {
        CanDealDamage = true;
        animator.SetBool("canStart", true);
        RB2D.AddForce(speed/2 * Time.deltaTime * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) , ForceMode2D.Force);
    }

    void DestroyAction()
    {
        animator.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        RB2D.isKinematic = true;
        RB2D.velocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
        destroyPE.gameObject.SetActive(true);
        Invoke(nameof(DestroyGameObj), 1f);
    }
    void DestroyGameObj()
    {
        Destroy(gameObject);
    }
}
