using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] protected Transform myTransform;
    [SerializeField] protected SpriteRenderer SR;

    public bool canLookAtPlayer = true;

    bool facingRight;

    // Update is called once per frame
    void Update()
    {
        FacePlayer();
        if (!facingRight)
        {
            SR.flipX = false;
        }
        else
        {
            SR.flipX = true;
        }
    }

    protected void FacePlayer()
    {
        if (canLookAtPlayer)
        {
            if (PlayerMovement.instance.GetTransform().position.x < myTransform.position.x && facingRight == false)
            {
                Flip();
            }
            else if (PlayerMovement.instance.GetTransform().position.x >= myTransform.position.x && facingRight == true)
            {
                Flip();
            }
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
    }
}
