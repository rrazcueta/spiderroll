using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    bool setup;
    GameObject source;
    float timeEnabled;
    public float lifeTime = 2;
    public GameObject destroyEffect;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        setup = true;
    }

    void Update()
    {
        if (timeEnabled + lifeTime < Time.time)
            Deactivate();
    }

    public void Shoot(Vector3 position, Vector3 direction, GameObject source)
    {
        gameObject.SetActive(true);
        transform.position = position;
        this.source = source;
        timeEnabled = Time.time;

        if (!setup)
            Awake();

        rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject == this.source)
            return;

        LocationDamage locationDamage = co.gameObject.GetComponent<LocationDamage>();
        if (locationDamage)
            locationDamage.TakeDamage();

        CharacterDamage characterDamage = co.gameObject.GetComponent<CharacterDamage>();
        if (characterDamage)
            characterDamage.SetInjured();

        Enemy enemy = co.gameObject.GetComponent<Enemy>();
        if (enemy)
            enemy.TakeDamage();

        Debug.Log("COLLIDED WITH: " + co.gameObject.name);

        if (!(co.gameObject.name == "Bullet"))
            Deactivate();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
        BulletPool.BP.availableBullets.Push(gameObject);

        if (destroyEffect)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }
}
