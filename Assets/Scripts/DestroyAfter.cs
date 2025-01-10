using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time;
    public bool animationCompletes;
    Animator animator;

    public float speedOffset;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (!animator)
            animator = GetComponentInChildren<Animator>();
        if (animator)
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);

        if (time > 0)
            Destroy(gameObject, time);
        else if (!animator)
            Debug.Log("Gameobject will not be destroyed");

        animator.speed = 1 - speedOffset / 2 + Random.Range(0, speedOffset);
    }
}
