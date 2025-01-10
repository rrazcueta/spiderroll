using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField]
    Vector3 rotate;

    void Update()
    {
        Vector3 rotateThisStep = rotate * Time.deltaTime;
        transform.Rotate(rotateThisStep.x, rotateThisStep.y, rotateThisStep.z, Space.Self);
    }
}
