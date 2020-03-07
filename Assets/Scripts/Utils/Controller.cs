using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public KeyCode forward = KeyCode.W, backward = KeyCode.S, left = KeyCode.A, right = KeyCode.D;
    public float mouseSpeedX, mouseSpeedY, moveSpeed, minYRot, maxYRot, minXRot, maxXRot, deathWait = 30;
    public Rigidbody rb;
    public bool isMoving, isAlive = true;
    public Transform theCam;
    public Transform body;

    protected bool shouldMove = false;
    protected Timer deathTimer;

    protected virtual void Start()
    {
        SetCursorProperties();
        deathTimer = Instantiate<Timer>(new Timer(30, GotAlive, false));
    }

    private void GotAlive()
    {
        isAlive = true;
    }

    private static void SetCursorProperties()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    protected virtual void Update()
    {
        CheckInput();
    }

    protected virtual void CheckInput()
    {
        Vector3 moveDir = Vector3.zero;
        shouldMove = false;

        if (Input.GetKey(forward))
        {
            moveDir += body.forward;
            shouldMove = true;
        }

        if (Input.GetKey(backward))
        {
            moveDir -= body.forward;
            shouldMove = true;
        }

        if (Input.GetKey(left))
        {
            moveDir -= body.right;
            shouldMove = true;
        }

        if (Input.GetKey(right))
        {
            moveDir += body.right;
            shouldMove = true;
        }

        if (shouldMove)
        {
            Move(moveDir.normalized);
        }
        else
        {
            StopMoving();
        }
    }

    protected virtual void Move(Vector3 moveVector)
    {
        rb.velocity = moveVector * moveSpeed;
    }

    protected virtual void StopMoving()
    {
        rb.velocity = rb.velocity - rb.velocity * Time.deltaTime * moveSpeed;
    }

    protected virtual void FixedUpdate()
    {
        float rx = Input.GetAxis("Mouse Y") * mouseSpeedX;
        float ry = Input.GetAxis("Mouse X") * mouseSpeedY;

        theCam.Rotate(new Vector3(-rx, 0, 0) * Time.fixedDeltaTime);
        transform.Rotate(body.up * ry * Time.fixedDeltaTime);
    }
}
