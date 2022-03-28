using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    public static LookAtMouse instance;
    Vector3 mousePosition;
    Camera cam;
    Transform myTransform;
    public bool canLookMouse;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
        myTransform = transform;
        canLookMouse = true;
    }

    void FixedUpdate()
    {
        if(canLookMouse)
            Look();
    }
    
    void Look()
    {
        mousePosition = Input.mousePosition; ;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0;
        myTransform.up = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
    }
}
