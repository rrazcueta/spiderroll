using UnityEngine;

public class DumbFollow : MonoBehaviour
{
    [SerializeField]
    Transform toFollow;

    [SerializeField]
    float distance;

    void FixedUpdate(){
        if(toFollow)
            MoveToward(toFollow);
    }

    void MoveToward(Transform t){
        if((transform.position - t.position).sqrMagnitude < distance * distance)
            return;
        
        transform.LookAt(t.position);

        transform.position = t.position - distance * transform.forward;
    }
}
