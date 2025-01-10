using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool BP;

    public Stack<GameObject> availableBullets = new();
    public Stack<GameObject> bulletsInUse = new();

    public GameObject bullet;
    public int startingCount;

    public bool shoot;

    void Awake()
    {
        if (!BP)
        {
            BP = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < startingCount; i++)
        {
            CreateNewBullet();
        }
    }

    void Update()
    {
        if (shoot)
        {
            shoot = false;
            Shoot(transform.position, Vector3.right, null);
        }
    }

    void CreateNewBullet()
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.SetActive(false);
        newBullet.transform.SetParent(transform, false);
        availableBullets.Push(newBullet);
    }

    public static void StaticShoot(Vector3 position, Vector3 direction, GameObject source)
    {
        BP.Shoot(position, direction, source);
    }

    void Shoot(Vector3 position, Vector3 direction, GameObject source)
    {
        if (availableBullets.Count <= 0)
            CreateNewBullet();

        GameObject bulletToShoot = availableBullets.Pop();
        bulletsInUse.Push(bulletToShoot);

        bulletToShoot.GetComponent<Bullet>().Shoot(position, direction, source);
    }
}
