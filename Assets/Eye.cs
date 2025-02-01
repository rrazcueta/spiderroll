using UnityEngine;

public class Eye : MonoBehaviour
{
    float nextBlinkTime = 0;
    float timeToNextBlinkMin = 1;
    float timeToNextBlinkMax = 5;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time > nextBlinkTime)
        {
            anim.speed = Random.Range(0.5f, 1.5f);
            anim.Play("Eye Blink", 0, 0);
            nextBlinkTime = Time.time + Random.Range(timeToNextBlinkMin, timeToNextBlinkMax);
        }
    }
}
