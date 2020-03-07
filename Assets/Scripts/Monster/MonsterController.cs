using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : Controller
{
    public KeyCode ChangeWallKey = KeyCode.Space;
    public DummyAnimations animations;
    public float wallRotation;
    public int xRotsCount;
    public Sensors sensors;

    public Vector3 gravityDir = Vector3.down;
    public float gravityScale = 40;

    bool canGoOnWall = false;

    protected override void Start()
    {
        animations = GetComponent<DummyAnimations>();
        sensors.sensorsDic["Front"].OnTriggered += ActivateCGW;
        sensors.sensorsDic["Front"].OnUntriggered += DeactivateCGW;
    }

    protected override void CheckInput()
    {
        base.CheckInput();

        if (shouldMove)
        {
            if (!animations.walking) animations.Walk();
        }
        else
        {
            if (animations.walking) animations.Idle();
        }

        if(canGoOnWall && Input.GetKeyDown(ChangeWallKey))
        {
            GoOnWall();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.velocity += gravityDir * gravityScale * Time.fixedDeltaTime;
    }

    private void GoOnWall()
    {
        body.Rotate(wallRotation, 0, 0);
        gravityDir = -MapVectorToDim(body.up);
    }

    private Vector3 SnapTo90s(Vector3 eulerAngs)
    {
        float ddiff = eulerAngs.x % 90;
        float udiff = (eulerAngs.x + 90) % 90;
        float diff = Mathf.Abs(ddiff) < Mathf.Abs(udiff) ? ddiff : udiff;

        eulerAngs.x -= diff;

        ddiff = eulerAngs.y % 90;
        udiff = (eulerAngs.y + 90) % 90;
        diff = Mathf.Abs(ddiff) < Mathf.Abs(udiff) ? ddiff : udiff;

        eulerAngs.y -= diff;

        ddiff = eulerAngs.z % 90;
        udiff = (eulerAngs.z + 90) % 90;
        diff = Mathf.Abs(ddiff) < Mathf.Abs(udiff) ? ddiff : udiff;

        eulerAngs.z -= diff;

        return eulerAngs;
    }

    private Vector3 MapVectorToDim(Vector3 vector)
    {
        if(Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            vector.y = 0;

            if(Mathf.Abs(vector.x) > Mathf.Abs(vector.z))
            {
                vector.z = 0;
            }
            else
            {
                vector.x = 0;
            }
        }
        else
        {
            vector.x = 0;

            if (Mathf.Abs(vector.y) > Mathf.Abs(vector.z))
            {
                vector.z = 0;
            }
            else
            {
                vector.y = 0;
            }
        }

        return vector.normalized;
    }

    private void ActivateCGW()
    {
        canGoOnWall = true;
    }

    private void DeactivateCGW()
    {
        canGoOnWall = false;
    }
}
