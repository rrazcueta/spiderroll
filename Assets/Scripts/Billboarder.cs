using UnityEngine;

public class Billboarder : MonoBehaviour
{
    Transform camTarget;
    public bool stop;
    static bool STOP;

    void Awake(){
        if(!camTarget) camTarget = Camera.main.transform.parent.parent;
    }

    void Stop(){
        stop = false;
        STOP = !STOP;
    }

    void LateUpdate(){
        if(stop) Stop();
        if(STOP) return;
        if(!camTarget) return;
        
        transform.LookAt(transform.position + camTarget.forward);
    }
}
