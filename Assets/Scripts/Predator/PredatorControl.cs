using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// TODO
/// 1- make it stick to the walls
/// 2- make it walk on the ceiling
/// 3- make it be able to disapear kinda
/// 4- make it able to turn to an object xD
///


public class PredatorControl : Controller
{
    public KeyCode jump = KeyCode.Space;
    public float jumpSpeed;
    public Sensors sensors;

    bool onGround = true;
    bool onWall = false;

    VictimSystem vs;
    MultiplayerSystem ms;

    protected override void Start()
    {
        base.Start();

        SetSensorsActions();

        vs = GetComponentInChildren<VictimSystem>();
        ms = FindObjectOfType<MultiplayerSystem>();
    }

    private void SetSensorsActions()
    {
        sensors.sensorsDic["Down"].OnTriggered += ActivateOnGround;
        sensors.sensorsDic["Down"].OnUntriggered += DeactivateOnGround;

        sensors.sensorsDic["Surrounding"].OnTriggered += ActivateOnWall;
        sensors.sensorsDic["Surrounding"].OnUntriggered += DeactivateOnWall;
    }

    protected override void Update()
    {
        base.Update();

        if (onWall)
        {
            ClampFallSpeed();
        }
    }

    protected override void CheckInput()
    {
        base.CheckInput();

        if (Input.GetKeyDown(jump) && (onGround || onWall))
        {
            Jump();
        }

        if(Input.GetMouseButtonDown(0) && vs.hasTarget)
        {
            ms.SendScarePacket(vs.targetIndex);
        }
    }

    private void ClampFallSpeed()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y > 0 ? rb.velocity.y : 0, rb.velocity.z);
    }

    private void Jump()
    {
        DeactivateOnGround();
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
    }

    private void ActivateOnGround()
    {
        onGround = true;
    }

    private void DeactivateOnGround()
    {
        onGround = false;
    }

    private void ActivateOnWall()
    {
        onWall = true;
        rb.useGravity = false;
    }

    private void DeactivateOnWall()
    {
        onWall = false;
        rb.useGravity = true;
    }
}
