using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    public GameObject sub;

    void Awake()
    {
        sub.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
    }

    void Update()
    {
        if (sub == null)
            Destroy(gameObject);
    }
}
