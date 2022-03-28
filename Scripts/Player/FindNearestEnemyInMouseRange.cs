using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestEnemyInMouseRange : MonoBehaviour
{
    public static FindNearestEnemyInMouseRange instance;
    [SerializeField] Collider2D[] enemiesToDamage;
    [SerializeField] Camera mainCam;
    [SerializeField] float detectRange;
    [SerializeField] LayerMask whatIsEnemy;

    [SerializeField] Transform NearestEnemy;
    [SerializeField] float closestDistance;
    [SerializeField] float currentDistance = 0f;

    [SerializeField] Transform ringPb;
    [SerializeField] Transform ringPb_2;
    Transform ringTf;

    Vector2 mousePos;
    Vector3 playerPos;

    private void Awake()
    {
        instance = this;
    }

    public void SetUpRing(float radius, int index)
    {
        switch (index)
        {
            case 0:
                ringTf = Instantiate(ringPb);
                break;
            case 1:
                ringTf = Instantiate(ringPb_2);
                break;
        }
        
        detectRange = radius;
        ringTf.localScale *= detectRange;
    }

    private void Update()
    {
        playerPos = PlayerMovement.instance.GetTransform().position;
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        enemiesToDamage = Physics2D.OverlapCircleAll(mousePos, detectRange, whatIsEnemy);
        FindNearestEnemy();
    }

    public Transform FindNearestEnemy()
    {
        if(enemiesToDamage.Length > 0)
        {
            closestDistance = Vector2.Distance(enemiesToDamage[0].transform.position, playerPos);
            NearestEnemy = enemiesToDamage[0].transform;
            foreach(Collider2D enemyCol in enemiesToDamage)
            {
                currentDistance = Vector2.Distance(enemyCol.transform.position, playerPos);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    NearestEnemy = enemyCol.transform;
                }
            }
        }
        else
        {
            NearestEnemy = null;
        }

        return NearestEnemy;
    }

    public Transform GetNearestEnemy()
    {
        return NearestEnemy;
    }

    public Vector2 GetMousePos()
    {
        return mousePos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mousePos, detectRange);
    }
}
