using UnityEngine;

[System.Serializable]
public class SimpleTimer
{
    float lastActivated = 0;

    float delay;

    public SimpleTimer(float delay)
    {
        lastActivated = Time.time;
        this.delay = delay;
    }

    public bool TryUse()
    {
        if (lastActivated + delay > Time.time)
            return false;

        Use();
        return true;
    }

    public bool CanUse() => lastActivated + delay < Time.time;

    public void Use() => lastActivated = Time.time;
}
