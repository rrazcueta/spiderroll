using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int cumulativeDamage;
    public int maxHealth = 3;
    public Rigidbody rb;
    float liveUntil;
    public float lifeTime;
    public float acceleration;
    public float maxSpeed;

    public static int COUNT = 0;

    Material healthyMaterial;
    public Material hitflashMaterial;
    float hitFlashUntil;
    float hitFlashTime = 2 / 60f;
    MeshRenderer mr;

    public float cohesionRadius;
    public float cohesionScale;
    public float alignmentRadius;
    public float alignmentScale;
    public float separationRadius;
    public float separationScale;
    public Vector3 totalSeparationForces;
    float sinOffset;

    public float maxBoidsForceSqr;

    // public Vector3 targetVelocity;

    public GameObject destroyEffect;

    static List<GameObject> ROLLIES = new List<GameObject>();

    public static void RESET()
    {
        ROLLIES = new List<GameObject>();
        COUNT = 0;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        healthyMaterial = mr.material;
        liveUntil = Time.time + lifeTime;
        COUNT++;

        ROLLIES.Add(gameObject);

        sinOffset = Random.Range(0, Mathf.PI);

        cohesionRadius += Random.Range(0, 2f) - 1;
        alignmentRadius += Random.Range(0, 2f) - 1;
        separationRadius += Random.Range(0, 2f) - 1;
    }

    void Update()
    {
        if (Time.time < hitFlashUntil)
            mr.material = hitflashMaterial;
        else
            mr.material = healthyMaterial;

        if (Time.time > liveUntil)
            DestroyThis();
    }

    void FixedUpdate()
    {
        QuickBoids();
        RollTowardsNearest();
        ClampXZVelocity();
    }

    void QuickBoids()
    {
        if (ROLLIES.Count == 0)
        {
            Debug.Log("ROLLIES COUNT IS ZERO");
            return;
        }

        Vector3 totalPosition = Vector3.zero;
        Vector3 totalVelocity = Vector3.zero;
        totalSeparationForces = Vector3.zero;

        int cohesionCount = 0;
        int alignmentCount = 0;
        int separationCount = 0;

        foreach (GameObject boid in ROLLIES)
        {
            if (boid == gameObject)
                continue;

            if (boid == null)
            {
                Debug.Log("Found null rollie");
                continue;
            }

            Vector3 vDiff = transform.position - boid.transform.position;
            float diff = vDiff.sqrMagnitude;

            //Cohesion
            if (diff < cohesionRadius * cohesionRadius)
            {
                totalPosition += boid.transform.position;
                cohesionCount++;
            }

            //Alignment
            Rigidbody boidRb = boid.GetComponent<Rigidbody>();
            if (diff < alignmentRadius * alignmentRadius)
            {
                totalVelocity += boidRb.velocity;
                alignmentCount++;
            }

            float sep = separationRadius + Mathf.Sin(Time.time + sinOffset) * 2;
            //Separation
            if (diff < sep * sep)
            {
                totalSeparationForces += vDiff;
                separationCount++;
            }
        }

        Vector3 averagePosition =
            cohesionCount > 0 ? totalPosition / cohesionCount : transform.position;

        Vector3 averageVelocity = alignmentCount > 0 ? totalVelocity / alignmentCount : rb.velocity;

        Vector3 cohesionVector = (averagePosition - transform.position) * cohesionScale;
        Vector3 alignmentVector = (averageVelocity - rb.velocity) * alignmentScale;
        Vector3 separationVector = totalSeparationForces * separationScale;

        Vector3 total = cohesionVector + alignmentVector + separationVector;

        maxBoidsForceSqr = Mathf.Max(maxBoidsForceSqr, total.sqrMagnitude);

        rb.AddForce(total, ForceMode.Acceleration);
    }

    void RollTowardsNearest()
    {
        if (!rb)
            return;

        // List<GameObject> playerObjects = GameManager.GM.healthyPlayerObjects; //MAKE A LIST OF NON-INJURED PLAYERS
        List<GameObject> playerObjects = GameManager.GM.healthyPlayerObjects; //MAKE A LIST OF NON-INJURED PLAYERS

        if (playerObjects == null || playerObjects.Count == 0)
            return;

        GameObject nearest = playerObjects[0];
        float toNearestSqrMagnitude = (
            transform.position - nearest.transform.position
        ).sqrMagnitude;
        foreach (GameObject player in playerObjects)
        {
            float sqrMagnitude = (transform.position - player.transform.position).sqrMagnitude;
            if (sqrMagnitude < toNearestSqrMagnitude)
            {
                nearest = player;
                toNearestSqrMagnitude = sqrMagnitude;
            }
        }

        Vector3 direction = (nearest.transform.position - transform.position).normalized;

        rb.AddForce(direction * acceleration, ForceMode.Acceleration);
    }

    void ClampXZVelocity()
    {
        Vector2 xz = new Vector2(rb.velocity.x, rb.velocity.z);

        if (xz.sqrMagnitude > maxSpeed * maxSpeed)
        {
            xz.Normalize();
            rb.velocity = new Vector3(xz.x * 6, rb.velocity.y, xz.y * 6);
        }
    }

    public void TakeDamage()
    {
        hitFlashUntil = Time.time + hitFlashTime;
        cumulativeDamage++;
        CheckDeath();
    }

    void CheckDeath()
    {
        if (cumulativeDamage >= maxHealth)
            DestroyThis();
    }

    void OnCollisionEnter(Collision co)
    {
        CharacterDamage characterDamage = co.gameObject.GetComponent<CharacterDamage>();
        if (characterDamage)
        {
            characterDamage.SetInjured();
            DestroyThis();
        }
    }

    void DestroyThis()
    {
        COUNT--;
        if (!ROLLIES.Contains(gameObject))
            Debug.Log("How did this rolly not get listed??");
        else
            ROLLIES.Remove(gameObject);

        if (destroyEffect)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
