using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationDamage : MonoBehaviour
{
    [SerializeField]
    int damageRate;

    [SerializeField]
    int cumulativeDamage;

    Boss boss;

    MeshRenderer mr;
    Material healthyMaterial;
    public float damagedThreshold;
    public Material damagedMaterial;

    [SerializeField]
    Material hitflashMaterial;
    float hitFlashUntil;

    static float DAMAGE_TIME = 1 / 60f;

    void Awake()
    {
        boss = GetComponentInParent<Boss>();
        mr = GetComponent<MeshRenderer>();
        healthyMaterial = mr.material;
    }

    void Update()
    {
        if (Time.time < hitFlashUntil)
            mr.material = hitflashMaterial;
        else
        {
            bool useDamaged = damagedThreshold > 0 && !!damagedMaterial;

            if (!useDamaged)
                mr.material = healthyMaterial;
            else
                mr.material =
                    damagedThreshold > cumulativeDamage ? healthyMaterial : damagedMaterial;
        }
    }

    public void ResetCumulativeDamage()
    {
        cumulativeDamage = 0;
    }

    public void TakeDamage()
    {
        cumulativeDamage += damageRate;
        hitFlashUntil = Time.time + damageRate * DAMAGE_TIME;
        boss.TakeDamage(damageRate);
    }

    public bool Broken()
    {
        if (damagedThreshold == 0)
            return false;

        return damagedThreshold <= cumulativeDamage;
    }
}
