using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public bool move;
    public bool airMove;
    public bool safeWalk;
    public float speed;
    public Vector3[] lastPos = new Vector3[10];

    [SerializeField]
    float walkSpeed;

    [SerializeField]
    float maximumSpeed;

    [SerializeField]
    float brakeDelay;

    [SerializeField]
    float dashPower;

    [SerializeField]
    float rotateSpeed;

    [SerializeField]
    float backflipSpeed;

    [Range(0f, 1f)]
    [SerializeField]
    float backflipDashMomentum;

    [SerializeField]
    float jumpSpeed;

    [SerializeField]
    float bigJumpSpeed;

    [Range(0f, 1f)]
    [SerializeField]
    float airControl;
    bool loseAirControl;

    Rigidbody rb;
    Vector3 relativeMotion;
    CapsuleCollider capCol;
    public bool ice;

    [SerializeField]
    float iceDashDelay;
    bool water;

    Animator animator;

    [SerializeField]
    public bool isControllable;

    [SerializeField]
    public bool isOnTheGround;

    [SerializeField]
    public bool isDashing;
    float lastDashTime;
    float lastBreakEffect;
    Transform cameraYaw;
    bool movedThisFrame;
    bool rotatedThisFrame;
    bool jumpedThisFrame;
    float autoJump;
    Vector3 lastMove;

    [SerializeField]
    GameObject runEffect;

    [SerializeField]
    GameObject brakeEffect;

    [SerializeField]
    float breakEffectDelay;

    [SerializeField]
    GameObject backflipEffect;

    [SerializeField]
    GameObject jumpEffect;

    [SerializeField]
    GameObject bigJumpEffect;
    Spawner spawner;

    [SerializeField]
    float highDrag;

    [SerializeField]
    float lowDrag;

    [SerializeField]
    PhysicsMaterial lowFriction;

    [SerializeField]
    PhysicsMaterial normalFriction;

    bool canDunk;
    public Transform gun;

    void Awake()
    {
        cameraYaw = Camera.main.transform.parent.parent;
        rb = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        spawner = GetComponent<Spawner>();

        for (int i = 0; i < lastPos.Length; i++)
        {
            lastPos[i] = transform.position;
        }
    }

    void Update() { }

    void FixedUpdate()
    {
        if (Time.fixedDeltaTime > 0)
        {
            speed = 0;
            Vector3 curPos = transform.position;
            Vector3 prevPos = lastPos[0];
            for (int i = 0; i < lastPos.Length; i++)
            {
                prevPos = lastPos[i];
                speed += (curPos - prevPos).magnitude / Time.fixedDeltaTime;

                lastPos[i] = curPos;
                curPos = prevPos;
            }
            speed *= 1f / lastPos.Length;
        }

        // var accumulatedTorque = rb.GetAccumulatedTorque();

        animator.SetBool("Walking", movedThisFrame);
        movedThisFrame = false;
        rotatedThisFrame = false;
        jumpedThisFrame = false;
        CheckGround();
        DashPhysics();
    }

    void CheckGround()
    {
        bool wasOnTheGround = isOnTheGround;
        Vector3 sphereCastStart = transform.position + capCol.center;
        RaycastHit hitInfo;
        isOnTheGround = Physics.SphereCast(
            sphereCastStart,
            capCol.radius,
            Vector3.down,
            out hitInfo,
            capCol.height / 2 + 0.1f
        );

        if (isOnTheGround)
        {
            loseAirControl = false;
        }

        if (isOnTheGround && hitInfo.rigidbody)
        {
            relativeMotion = hitInfo.rigidbody.GetPointVelocity(hitInfo.point);
        }
        else
        {
            relativeMotion = Vector3.zero;
        }

        ice = isOnTheGround && hitInfo.collider.gameObject.tag == "Ice";

        if (autoJump > 0)
        {
            if (wasOnTheGround && !isOnTheGround)
            {
                Jump(lastMove);
                autoJump = 0;
            }
            autoJump -= Time.fixedDeltaTime;
        }

        animator.SetBool("OnTheGround", isOnTheGround);
    }

    Vector3 RelativeDirection(Vector2 input)
    {
        Transform relativeTo = cameraYaw ? cameraYaw : transform;
        return relativeTo.right * input.x + relativeTo.forward * input.y;
    }

    public void Move(Vector2 input)
    {
        if (movedThisFrame)
            return;
        if (input.sqrMagnitude <= 0)
            return;
        movedThisFrame = true;

        lastMove = input;
        float maxWalkSpeed = ice ? 2f : walkSpeed;

        Vector3 moveDirection = RelativeDirection(input);

        float rbVelocityRatio =
            1
            - Mathf.Min(1, (new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z) / maximumSpeed).magnitude);
        float horizontalStep = maxWalkSpeed * Time.fixedDeltaTime * rbVelocityRatio;
        Vector3 newPosition = rb.position + moveDirection * horizontalStep;

        float verticalStep = horizontalStep + 0.1f; //TODO do the trig to figure out the right angle

        RaycastHit hit;
        bool walkIsSafe = Physics.Raycast(
            newPosition + capCol.center,
            Vector3.down,
            out hit,
            capCol.height / 2 + verticalStep
        );

        if (safeWalk && !walkIsSafe)
        {
            return;
        }

        rb.MovePosition(newPosition);
    }

    public void AirMove(Vector2 input)
    {
        if (loseAirControl)
            return;

        if (movedThisFrame)
            return;
        if (input.sqrMagnitude <= 0)
            return;
        movedThisFrame = true;

        Vector3 moveDirection = RelativeDirection(input);

        float rbVelocityRatio =
            1
            - Mathf.Min(1, (new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z) / maximumSpeed).magnitude);
        float horizontalStep = walkSpeed * airControl * Time.fixedDeltaTime * rbVelocityRatio;
        Vector3 newPosition = rb.position + moveDirection * horizontalStep;
        rb.MovePosition(newPosition);
    }

    public void Dunk()
    {
        if (loseAirControl)
            return;
        if (!canDunk)
            return;

        float newY = Mathf.Min(-bigJumpSpeed, rb.linearVelocity.y);

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, newY, rb.linearVelocity.z);
        // rb.velocity = new Vector3(rb.velocity.x / 2, newY, rb.velocity.z / 2);
        // rb.velocity = new Vector3(0, newY, 0);

        animator.Play("Dunk");
    }

    public void Rotate(Vector2 input)
    {
        if (rotatedThisFrame)
            return;
        if (input.sqrMagnitude <= 0)
            return;
        rotatedThisFrame = true;

        Vector3 moveDirection = RelativeDirection(input);

        Vector3 newDirection = Vector3.RotateTowards(
            transform.forward,
            moveDirection,
            Mathf.Deg2Rad * rotateSpeed * Time.fixedDeltaTime,
            0
        );
        transform.LookAt(transform.position + newDirection);
    }

    public void QuickRotate(Vector2 input)
    {
        if (input.sqrMagnitude <= 0)
            return;

        Vector3 moveDirection = RelativeDirection(input);

        transform.LookAt(transform.position + moveDirection);
    }

    // public void AimGun(Vector2 input)
    // {
    //     // if (aimedGunThisFrame)
    //     //     return;
    //     if (input.sqrMagnitude <= 0)
    //         return;

    //     aimedGunThisFrame = true;

    //     Vector3 aimDirection = RelativeDirection(input);

    //     Vector3 newDirection = Vector3.RotateTowards(
    //         gun.forward,
    //         aimDirection,
    //         Mathf.Deg2Rad * rotateSpeed * Time.fixedDeltaTime,
    //         0
    //     );
    //     gun.LookAt(transform.position + newDirection);
    // }

    public void Dash(Vector2 input)
    {
        if (ice && Time.time - lastDashTime < iceDashDelay)
            return;

        lastDashTime = Time.time;
        Transform relativeTo = cameraYaw ? cameraYaw : transform;

        Vector3 dashDirection = relativeTo.right * input.x + relativeTo.forward * input.y;
        Vector2 dashDirV2 = new Vector2(dashDirection.x, dashDirection.z);

        float dashStrength = dashPower * maximumSpeed * (ice ? 0.5f : 1);

        float xzVelocitySqrMagnitudeBefore = new Vector3(
            rb.linearVelocity.x,
            0,
            rb.linearVelocity.z
        ).sqrMagnitude;
        rb.linearVelocity += new Vector3(dashDirV2.x, 0, dashDirV2.y) * dashStrength;
        float xzVelocitySqrMagnitudeAfter = new Vector3(
            rb.linearVelocity.x,
            0,
            rb.linearVelocity.z
        ).sqrMagnitude;

        if (
            xzVelocitySqrMagnitudeAfter > Mathf.Pow(maximumSpeed, 2)
            && xzVelocitySqrMagnitudeAfter > xzVelocitySqrMagnitudeBefore
        )
        {
            float speed = Mathf.Max(Mathf.Pow(xzVelocitySqrMagnitudeBefore, 0.5f), maximumSpeed);
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }

        spawner.Spawn("run", -Vector3.up * 0.707107f, Quaternion.identity);
    }

    public void Brake()
    {
        lastDashTime = -brakeDelay;
    }

    void DashPhysics()
    {
        rb.useGravity = true;
        if (Time.time - lastDashTime <= brakeDelay && isOnTheGround)
        {
            capCol.material = lowFriction;
            rb.angularDamping = lowDrag;
            rb.useGravity = false;
            return;
        }

        if (rb.linearVelocity.y <= 0)
        {
            capCol.material = normalFriction;
            rb.angularDamping = highDrag;
        }

        bool isMovingXZ =
            new Vector3(
                rb.linearVelocity.x - relativeMotion.x,
                0,
                rb.linearVelocity.z - relativeMotion.z
            ).sqrMagnitude > walkSpeed;
        if (isOnTheGround && isMovingXZ && Time.time - lastBreakEffect > breakEffectDelay && !ice)
        {
            spawner.Spawn("brake", -Vector3.up * 0.707107f, Quaternion.identity);
            lastBreakEffect = Time.time;
        }
    }

    public void Backflip()
    {
        if (jumpedThisFrame)
            return;
        jumpedThisFrame = true;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x * backflipDashMomentum,
            backflipSpeed,
            rb.linearVelocity.z * backflipDashMomentum
        );

        spawner.Spawn("backflip", -Vector3.up * 0.707107f, Quaternion.identity);
        animator.Play("Backflip");
    }

    public void JumpPrep(float time)
    {
        lastDashTime = Time.time;
        autoJump = time;
    }

    public void Jump(Vector2 input)
    {
        if (jumpedThisFrame)
            return;
        jumpedThisFrame = true;
        if (!isOnTheGround)
            return;
        isOnTheGround = false;

        autoJump = 0;
        lastDashTime = Time.time;

        Vector3 moveDirection = RelativeDirection(input);

        Vector3 horizontal = new Vector3(
            moveDirection.x * walkSpeed * (1 - airControl),
            0,
            moveDirection.z * walkSpeed * (1 - airControl)
        );
        Vector3 vertical = Vector3.up * (jumpSpeed + relativeMotion.y);
        rb.linearVelocity += horizontal + vertical;

        if (rb.linearVelocity.y > jumpSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);

        spawner.Spawn("jump", -Vector3.up * 0.707107f, Quaternion.identity);
    }

    public void BigJump(Vector2 input)
    {
        if (jumpedThisFrame)
            return;
        jumpedThisFrame = true;
        if (!isOnTheGround)
            return;
        isOnTheGround = false;

        autoJump = 0;
        lastDashTime = Time.time;

        Vector3 moveDirection = RelativeDirection(input);

        Vector3 horizontal = new Vector3(
            moveDirection.x * walkSpeed * (1 - airControl),
            0,
            moveDirection.z * walkSpeed * (1 - airControl)
        );
        Vector3 vertical = Vector3.up * (bigJumpSpeed + relativeMotion.y);
        rb.linearVelocity += horizontal + vertical;

        if (rb.linearVelocity.y > bigJumpSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, bigJumpSpeed, rb.linearVelocity.z);

        canDunk = true;

        spawner.Spawn("bigJump", -Vector3.up * 0.707107f, Quaternion.identity);
        animator.Play("Frontflip");
    }

    public Vector2 GetHorizontalVelocity()
    {
        return new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
    }

    void OnCollisionEnter()
    {
        if (isOnTheGround)
            loseAirControl = true;

        canDunk = false;
    }
}
