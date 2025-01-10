using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject follow;
    public static CameraControl CAMCONTROL;

    void Awake()
    {
        if (!CAMCONTROL)
            CAMCONTROL = this;
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        return;
        if (follow)
        {
            transform.position = follow.transform.position;
        }
    }

    public static void RotateStatic(float amount)
    {
        if (!CAMCONTROL)
            return;

        CAMCONTROL.Rotate(amount);
    }

    void Rotate(float amount)
    {
        transform.Rotate(UnityEngine.Vector3.up, amount);
    }
}
