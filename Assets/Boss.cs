using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    //1 Player 1250
    //2 Player 1500
    //4 Player 2000
    //8 Player 3000
    [SerializeField]
    int baseHealth = 1000;

    [SerializeField]
    int bonusHealthByPlayer = 250;
    public float speed = 3;

    public float nearbyDistance = 16;

    [SerializeField]
    int cumulativeDamage;
    List<LocationDamage> locationDamages;

    public GameObject enemyToSpawn;
    public GameObject largeEnemy;

    [System.Serializable]
    public enum BossState
    {
        Idle,
        Walk,
        RotateRight,
        RotateLeft,
        StompInPlace,
        BrokenLegs,
        Laser
    }

    public BossState bossState;

    Animator anim;

    public LocationDamage[] legs;
    public bool breakD;

    [SerializeField]
    float rotationSpeed;

    float timeToChangeState;
    MeshExplosions meshExplosions;
    float birthdate;

    [SerializeField]
    float turnMin;

    [SerializeField]
    float turnMax;

    [SerializeField]
    float idleMin;

    [SerializeField]
    float idleMax;

    [SerializeField]
    float walkMin;

    [SerializeField]
    float walkMax;

    void Awake()
    {
        locationDamages = ComponentHelper.GetComponentsInAllChildren<LocationDamage>(transform);
        anim = GetComponent<Animator>();
        meshExplosions = GetComponent<MeshExplosions>();
        birthdate = Time.time;
    }

    void Update()
    {
        Legs();

        if (timeToChangeState < Time.time)
        {
            ChooseNewState();
        }

        switch (bossState)
        {
            case BossState.RotateRight:
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
                break;
            case BossState.RotateLeft:
                transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
                break;
            case BossState.Walk:
                transform.position += Time.deltaTime * speed * transform.forward;
                break;
            default:
                break;
        }
    }

    void ChooseNewState()
    {
        Vector3 camCenter = CameraControl.CAMCONTROL.transform.position;
        Vector3 currentLocation = transform.position;
        Vector3 distanceToCenter = currentLocation - camCenter;
        Vector3 heading = transform.position + transform.forward;
        Vector3 headingRight = transform.position + transform.forward + transform.right;
        Vector3 headingLeft = transform.position + transform.forward + transform.right;
        Vector3 headingToCenter = heading - camCenter;
        bool lookingTowardCenter = distanceToCenter.sqrMagnitude >= headingToCenter.sqrMagnitude;
        bool turnRight = headingRight.sqrMagnitude <= headingLeft.sqrMagnitude;

        Vector3 playerCenter = Vector3.zero;

        int nearbyPlayers = 0;
        foreach (GameObject player in GameManager.GM.healthyPlayerObjects)
        {
            playerCenter += player.transform.position;

            Vector3 toPlayer = player.transform.position - transform.position;
            float nearbySqrMagnitude = nearbyDistance * nearbyDistance;
            if (player.GetComponent<CharacterDamage>().injured)
                continue;
            if (toPlayer.sqrMagnitude > nearbySqrMagnitude)
                continue;

            nearbyPlayers++;
        }
        bool lookingAtPlayers = false;
        if (GameManager.GM.healthyPlayerObjects.Count > 0)
        {
            playerCenter *= 1 / GameManager.GM.healthyPlayerObjects.Count;

            lookingAtPlayers =
                (transform.position - playerCenter).sqrMagnitude
                >= (heading - playerCenter).sqrMagnitude;
        }

        BossState previousBS = bossState;

        //IDLE => TURN or WALK
        if (previousBS == BossState.Idle)
        {
            if (Random.Range(0, 1f) < 0.15f || !lookingAtPlayers)
                bossState = turnRight ? BossState.RotateRight : BossState.RotateLeft;
            else
                bossState = BossState.Walk;
        }
        //TURN => WALK or LASER or IDLE
        else if (previousBS == BossState.RotateLeft || bossState == BossState.RotateRight)
        {
            if (Random.Range(0, 1f) < 0.5f)
                bossState = BossState.Walk;
            else if (lookingAtPlayers && lookingTowardCenter)
                bossState = BossState.Laser;
            else
                bossState = BossState.Idle;
        }
        //LASER => WALK
        else if (previousBS == BossState.Laser)
        {
            bossState = BossState.Walk;
        }
        //STOMP, WALK => STOMP, LASER, WALK, RANDOM TURN or IDLE
        else
        {
            if (nearbyPlayers > 0)
                bossState = BossState.StompInPlace;
            else if (lookingTowardCenter || lookingAtPlayers)
                bossState = BossState.Laser;
            else
                bossState = Random.Range(0, 4) switch
                {
                    0 => BossState.RotateRight,
                    1 => BossState.RotateLeft,
                    _ => BossState.Idle
                };
        }

        if (
            !lookingTowardCenter
            && bossState == BossState.Walk
            && distanceToCenter.sqrMagnitude > 200
        )
        {
            bossState = turnRight ? BossState.RotateRight : BossState.RotateLeft;
        }

        // int randomState = Random.Range(0, 2);

        // bossState = randomState switch
        // {
        //     0 => turnRight ? BossState.RotateRight : BossState.RotateLeft,
        //     _ => BossState.Idle,
        // };

        // if (Random.Range(0, 1f) > 0.5f && birthdate + 5 < Time.time)
        // {
        //     if (nearbyPlayers > 0)
        //         bossState = BossState.StompInPlace;
        //     else if (lookingTowardCenter)
        //         bossState = Random.Range(0, 1f) > 0.5f ? BossState.Laser : BossState.Walk;
        //     else
        //         bossState = BossState.Idle;
        // }

        // bossState = BossState.Laser;

        if (bossState == BossState.Idle && Enemy.COUNT < 8 + GameManager.GM.playerObjects.Count * 2)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 randomSphere = Random.onUnitSphere;
                randomSphere = new Vector3(randomSphere.x, 0, randomSphere.z).normalized;
                Instantiate(
                    i % 3 == 0 ? largeEnemy : enemyToSpawn,
                    transform.position + -Vector3.up + randomSphere * 5,
                    Quaternion.identity
                );
            }
        }

        string stateAsString = bossState.ToString();
        timeToChangeState = Time.time + StateTime(bossState);

        anim.Play(stateAsString);
    }

    float StateTime(BossState s)
    {
        return s switch
        {
            BossState.BrokenLegs => 8,
            BossState.Laser => 5,
            BossState.StompInPlace => 3.5f,
            BossState.Walk => Random.Range(walkMin, walkMax),
            BossState.Idle => Random.Range(idleMin, idleMax),
            _ => Random.Range(turnMin, turnMax)
        };
    }

    void Legs()
    {
        int brokenLegs = 0;
        foreach (LocationDamage leg in legs)
            if (leg.Broken())
                brokenLegs++;

        if (brokenLegs >= 4)
            breakD = true;

        if (breakD)
        {
            Debug.Log("WE GOT SOME BROKEN BOSS LEGSS");
            breakD = false;
            bossState = BossState.BrokenLegs;
            anim.Play("LegBreakdown");
            AnimationClip currentClip = anim.runtimeAnimatorController.animationClips[0];
            timeToChangeState = Time.time + StateTime(bossState);
        }
    }

    public void TakeDamage(int amt)
    {
        cumulativeDamage += amt;
        UpdateHealth();
    }

    void UpdateHealth()
    {
        int currentHealth = MaxHealth() - cumulativeDamage;
        float percent = (float)currentHealth / MaxHealth();

        LevelManager.SET_HEALTH_BAR(percent);

        if (currentHealth <= 0)
        {
            Debug.Log("CURERNT HEALTH IS UNDER ZERO");
            this.enabled = false;
            meshExplosions.enabled = true;
            anim.Play("LegBreakdown");
            anim.SetBool("Dead", true);
            // GameManager.RESET();
            LevelManager.SET_TEXT("BOSS DEAD");
            LevelManager.SET_RESET_TIMER(10);
        }
    }

    int MaxHealth()
    {
        int multiplier = GameManager.GM ? GameManager.GM.playerCount : 1;
        return multiplier * bonusHealthByPlayer + baseHealth;
    }

    void RecoverLegs()
    {
        if (cumulativeDamage >= MaxHealth())
            return;

        foreach (LocationDamage leg in legs)
        {
            leg.ResetCumulativeDamage();
        }

        ChooseNewState();
    }
}
