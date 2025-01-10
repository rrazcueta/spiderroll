using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamp : MonoBehaviour
{
    [SerializeField]
    float xMin;

    [SerializeField]
    float xMax;

    [SerializeField]
    float zMin;

    [SerializeField]
    float zMax;

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if (pos.x < xMin)
        {
            pos = new Vector3(xMin, pos.y, pos.z);
        }
        else if (pos.x > xMax)
        {
            pos = new Vector3(xMax, pos.y, pos.z);
        }

        if (pos.z < zMin)
        {
            pos = new Vector3(pos.x, pos.y, zMin);
        }
        else if (pos.z > zMax)
        {
            pos = new Vector3(pos.x, pos.y, zMax);
        }
        transform.position = pos;
    }
}
