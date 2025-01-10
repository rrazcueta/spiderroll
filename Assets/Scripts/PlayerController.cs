using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{
    // public float accelMag,
    //     gyroMag;
    // public GameObject controller;
    // public Vector3 accelerometer;
    public Vector3 gyro;
    public Quaternion orientation;
    static int PLAYERCOUNT;
    int playerId;
    private Player player;
    CharacterMovement characterMovement;
    CharacterAction characterAction;
    public Vector2 analogStick;
    public Vector2 analogStickPrevious;
    public Vector2 rAnalogStickPrevious;
    public Vector2 rAnalogStick;
    public bool rAnalogStickTiltedPrevious;
    public bool rAnalogStickTilted;
    public float rAnalogThreshold;
    public Vector2 mouse;
    public float lastMouse;
    public float mouseThreshold;
    public bool dash;
    public float runCadence;
    float lastDashTime;
    Transform aim;
    float aimTarget;
    public float aimNear;
    public float aimMedium;
    public float aimFar;
    bool jump;
    bool startedJump;
    float lastPressedJump;
    public float timeToJump;

    public bool plantB;
    public bool blockB;
    public bool jumpB;
    public bool runB;
    public bool autoRun;
    public bool plantDown;

    public GameObject ui;
    public GameObject status;
    public GameObject inventory;
    public GameObject gun;
    public GameObject gunDown;
    float gunUpUntil;
    public float gunUpTime = .5f;

    void Awake()
    {
        playerId = PLAYERCOUNT;
        player = ReInput.players.GetPlayer(playerId);
        PLAYERCOUNT++;
        characterMovement = GetComponent<CharacterMovement>();
        characterAction = GetComponent<CharacterAction>();
        aim = transform.Find("Body").Find("Aim");
    }

    public static void RESET()
    {
        PLAYERCOUNT = 0;
    }

    void Start()
    {
        List<GameObject> players = GameManager.GM.playerObjects;
        List<GameObject> healthy = GameManager.GM.healthyPlayerObjects;

        if (players.Contains(gameObject) || healthy.Contains(gameObject))
            return;

        GameManager.GM.playerObjects.Add(gameObject);
        GameManager.GM.playerObjects.Add(gameObject);
    }

    void Update()
    {
        GetInput();

        bool gunUp = Time.time < gunUpUntil;
        gun.SetActive(gunUp);
        gunDown.SetActive(!gunUp);

        ShootGun();
    }

    void FixedUpdate()
    {
        Aim();

        if (!characterMovement.isControllable)
            return;

        // AimGun();

        if (characterMovement.isOnTheGround)
        {
            Move();
            Rotate();
            Dash();
            Brake();
            Run();
            Jump();
        }
        else
        {
            AirTurn();
            AirMove();
            Dunk();
            jump = startedJump = false;
        }

        dash = false;
    }

    private void GetInput()
    {
        plantB = player.GetButton("Plant");
        jumpB = player.GetButton("Jump");
        blockB = player.GetButton("Strafe");
        runB = player.GetButton("Run");

        plantDown = player.GetButtonDown("plant");

        analogStick = new Vector2(player.GetAxisRaw("Horizontal"), player.GetAxisRaw("Vertical"));
        if (analogStick.sqrMagnitude > 1)
            analogStick.Normalize();

        //THIS IS MOUSE FAKE ANALOG CONTROL... almost but not quite
        // if (analogStick.sqrMagnitude == 0)
        // {
        //     Vector2 tempMouse = new Vector2(player.GetAxis("MouseX"), player.GetAxis("MouseY"));
        //     if (tempMouse.sqrMagnitude > mouseThreshold)
        //     {
        //         lastMouse = Time.time;
        //         mouse = tempMouse.normalized;
        //     }
        // }

        if (!dash)
            dash = player.GetButtonDown("Run");
        if (!jump)
            jump = player.GetButtonDown("Jump");

        //GYRO GET
        // foreach (Joystick joystick in player.controllers.Joysticks)
        // {
        //     var ds = joystick.GetExtension<IDualShock4Extension>();
        //     if (ds == null)
        //         continue; // this is not a DS4, skip it

        //     // Get the accelerometer value
        //     accelerometer = ds.GetAccelerometerValue();
        //     accelMag = accelerometer.magnitude;

        //     // Get the gyroscope value
        //     gyro = ds.GetGyroscopeValue();
        //     gyroMag = gyro.magnitude;

        //     // Get the controller orientation
        //     orientation = ds.GetOrientation();

        //     if (controller != null)
        //     {
        //         // controller.transform.rotation = orientation;
        //         controller.transform.eulerAngles = new Vector3(
        //             orientation.eulerAngles.x,
        //             0,
        //             orientation.eulerAngles.z
        //         );
        //     }
        // }

        if (!blockB)
            analogStickPrevious = analogStick;

        rAnalogStickPrevious = rAnalogStick;
        rAnalogStickTiltedPrevious = rAnalogStickTilted;

        rAnalogStick = new Vector2(
            player.GetAxisRaw("RHorizontal"),
            player.GetAxisRaw("RVertical")
        );
        if (rAnalogStick.sqrMagnitude > 1)
            rAnalogStick.Normalize();
        rAnalogStickTilted = rAnalogStick.sqrMagnitude > rAnalogThreshold;
    }

    private void Aim()
    {
        //TODO simplify this

        float nextAim = 0;

        if (characterMovement.isControllable)
        {
            if (plantB && blockB)
                aimTarget = aimFar;
            else if (plantB)
                aimTarget = aimMedium;
            else if (blockB)
                aimTarget = aimNear;
            else
                aimTarget = 0;

            if (plantB || blockB)
                gunUpUntil = Time.time + gunUpTime;

            nextAim = (aim.localScale.y + aimTarget) / 2f;
        }

        aim.localScale = new Vector3(0.1f, nextAim, 1);
        aim.localPosition = new Vector3(0, 0, 0.5f + nextAim / 2);
    }

    // private void AimGun()
    // {
    //     if (plantB)
    //     {
    //         characterMovement.AimGun(analogStick);
    //         return;
    //     }

    //     characterMovement.AimGun(
    //         rAnalogStick.sqrMagnitude > 0 ? rAnalogStick : analogStickPrevious
    //     );
    //     return;

    //     if (rAnalogStick.sqrMagnitude > 0 || player.GetButton("Item0") && !blockB || plantB)
    //     {
    //         characterMovement.AimGun(rAnalogStick.sqrMagnitude > 0 ? rAnalogStick : analogStick);
    //         return;
    //     }

    //     float radians = 45 * Mathf.Deg2Rad;
    //     float cos = Mathf.Cos(radians);
    //     float sin = Mathf.Sin(radians);

    //     Vector2 tilted =
    //         new(
    //             analogStick.x * cos - analogStick.y * sin,
    //             analogStick.x * sin + analogStick.y * cos
    //         );

    //     characterMovement.AimGun(blockB ? analogStickPrevious : tilted);
    // }

    private void ShootGun()
    {
        if (!player.GetButton("Item0"))
            return;

        if (!blockB && analogStick.sqrMagnitude > 0 || rAnalogStick.sqrMagnitude > 0)
        {
            characterMovement.QuickRotate(
                rAnalogStick.sqrMagnitude > 0 ? rAnalogStick : analogStick
            );
        }

        gunUpUntil = Time.time + gunUpTime;

        characterAction.ShootGun();
    }

    private void Move()
    {
        if (analogStick.sqrMagnitude == 0)
            return;
        if (plantB)
            return;

        characterMovement.Move(analogStick);
    }

    private void AirTurn()
    {
        if (analogStick.sqrMagnitude <= 0 && rAnalogStick.sqrMagnitude <= 0)
            return;

        if (plantDown || player.GetButton("Item0"))
        {
            characterMovement.QuickRotate(
                rAnalogStick.sqrMagnitude > 0 ? rAnalogStick : analogStick
            );
        }
    }

    private void AirMove()
    {
        if (analogStick.sqrMagnitude == 0)
            return;
        if (plantB)
            return;

        characterMovement.AirMove(analogStick);
    }

    private void Dunk()
    {
        if (!jump)
            return;

        characterMovement.Dunk();
    }

    private void Rotate()
    {
        if (blockB)
            return;
        if (jumpB)
            return;

        if (analogStick.sqrMagnitude == 0 && rAnalogStick.sqrMagnitude == 0)
        {
            //     if (Time.time < lastMouse + 0.2f)
            //         characterMovement.Rotate(mouse);
            return;
        }

        if (rAnalogStick.sqrMagnitude > 0)
            gunUpUntil = Time.time + gunUpTime;

        Vector2 direction = rAnalogStick.sqrMagnitude > 0 ? rAnalogStick : analogStick;

        characterMovement.Rotate(direction);
    }

    private void Dash()
    {
        if (plantB)
            return;
        if (blockB)
            return;
        if (jumpB)
            return;
        if (analogStick.sqrMagnitude == 0)
            return;
        if (rAnalogStick.sqrMagnitude > 0)
            return;
        if (!dash)
            return;

        characterMovement.Dash(analogStick);
        lastDashTime = Time.time;
    }

    private void Brake()
    {
        if (!dash)
            return;
        if (analogStick.sqrMagnitude > 0)
            return;

        characterMovement.Brake();
        lastDashTime = Time.time;
    }

    private void Run()
    {
        //ACTIVATE AUTORUN
        if (
            !player.GetButtonDown("Run") // TODO??
            && runB
            && Time.time - lastDashTime > runCadence
        )
            autoRun = true;

        //BAIL
        if (Time.time - lastDashTime < runCadence)
            return;
        if (!autoRun)
            return;

        Vector2 velocity = characterMovement.GetHorizontalVelocity();
        bool movingAgainst = (velocity + analogStick).sqrMagnitude <= velocity.sqrMagnitude;

        //CANCEL AUTO RUN
        if (movingAgainst || plantB || blockB || jumpB || rAnalogStick.sqrMagnitude > 0)
        {
            autoRun = false;
            return;
        }

        characterMovement.Dash(analogStick);
        lastDashTime = Time.time;
    }

    private void Jump()
    {
        if (jump)
        {
            lastPressedJump = Time.time;
            startedJump = true;
        }

        if (!startedJump)
            return;

        if (jump)
        {
            if (analogStick.sqrMagnitude == 0)
            {
                characterMovement.Backflip();
                startedJump = false;
                jump = false;
                return;
            }
            else
            {
                characterMovement.JumpPrep(timeToJump);
            }
            jump = false;
        }

        if (Time.time - lastPressedJump > timeToJump)
        {
            characterMovement.BigJump(analogStick);
            startedJump = false;
            return;
        }

        if (!jumpB)
        {
            characterMovement.Jump(analogStick);
            startedJump = false;
            return;
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (!blockB)
            return;
        Block oBlock = col.gameObject.GetComponent<Block>();
        if (oBlock)
            oBlock.ConstrainRotation();
    }
}
