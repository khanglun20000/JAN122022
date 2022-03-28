using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearingFrame : MonoBehaviour
{
    [SerializeField] Animator animator;
    [HideInInspector] public NormalRoomController RC;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Anim_AppearingFrame");
    }

    void OnAnimationComplete()
    {
        RC.MonSpawned(transform);
        Destroy(gameObject);
    }
}