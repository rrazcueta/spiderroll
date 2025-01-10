using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : MonoBehaviour
{
    public Transform gunHead;

    float lastShotTime;
    public float timeBetweenShots = 0.1f;
    public float timeBetweenBursts = 0.5f;

    public void ShootGun()
    {
        if (Time.time - lastShotTime < timeBetweenBursts)
            return;

        lastShotTime = Time.time;
        StartCoroutine(BurstshotCR());
    }

    public IEnumerator BurstshotCR()
    {
        for (int i = 0; i < 3; i++)
        {
            BulletPool.StaticShoot(gunHead.position, gunHead.forward, gameObject);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
