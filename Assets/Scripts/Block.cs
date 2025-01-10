using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;
    float constrainRotation;
    [SerializeField] bool canConstrainRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public void ConstrainRotation()
    {
        if (!canConstrainRotation) return;
        constrainRotation = 2 * Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        if(!canConstrainRotation) return;
        if (constrainRotation > 0)
        {
            constrainRotation -= Time.fixedDeltaTime;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            constrainRotation = 0;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
