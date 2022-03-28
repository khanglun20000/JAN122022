using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;

    Camera cam;
    SpriteRenderer sr;

    Rigidbody2D rb;
    Vector2 cursorPos;

    public bool facingRight;

    public static PlayerMovement instance;
    Transform myTransform;
    float moveHorizontal;
    float moveVertical;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        myTransform = transform;
    }

    void FixedUpdate()
    {
        cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Move();

        if (facingRight == false && cursorPos.x >= myTransform.position.x)
        {
            Flip();
        }
        else if (facingRight == true && cursorPos.x < myTransform.position.x)
        {
            Flip();
        }

        if (facingRight)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    private void Move()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(moveHorizontal) >= 0.01f || Mathf.Abs(moveVertical) > 0.01f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        rb.MovePosition(rb.position + new Vector2(moveHorizontal, moveVertical).normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void Flip()
    {
        facingRight = !facingRight;
    }

    public Transform GetTransform()
    {
        return myTransform;
    }
}
