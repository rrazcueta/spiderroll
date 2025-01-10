using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject explosion;
    CapsuleCollider myCol;
    float cantExplodeUntil;
    public float explodeDelayMin = 0.0f;
    public float explodeDelayMax = 0.10f;
    public Transform center;

    void Awake()
    {
        myCol = GetComponent<CapsuleCollider>();
    }

    void OnTriggerStay(Collider co)
    {
        if (Time.time < cantExplodeUntil)
            return;

        if (co.gameObject.layer != 6)
            return;

        for (int i = 0; i < 4; i++)
        {
            bool hit = false;
            Vector3 startingPoint = Random.insideUnitSphere * 2.5f + center.position;
            LayerMask layer6Mask = 1 << 6;

            // bool hit = Physics.Raycast(startingPoint, transform.up, out info);
            hit = Physics.Raycast(startingPoint, transform.up, out RaycastHit info, 30, layer6Mask);

            Debug.Log("HIT INFO: " + hit + " & " + info.point);

            if (hit)
            {
                Instantiate(explosion, info.point, Quaternion.identity);
                cantExplodeUntil = Time.time + Random.Range(explodeDelayMin, explodeDelayMax);
            }
        }
    }
}
