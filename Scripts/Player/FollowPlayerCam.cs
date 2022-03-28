using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCam : MonoBehaviour
{
    public Transform target;

    private float smoothSpeed;

    public float startSmoothSpeed = 0.5f;

    private Vector3 velocity = Vector3.zero;

    public Vector3 _offset;

    private Vector3 desiredPosition;

    Vector3 smoothPosition;

    void FixedUpdate()
    {
        smoothSpeed = startSmoothSpeed;

        desiredPosition = target.transform.position + _offset;

        smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        transform.position = smoothPosition;
    }
}
