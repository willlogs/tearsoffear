using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public KeyCode forward, backward, left, right, jump, sprint, flashLightSwitch;
    public float mouseSpeedX, mouseSpeedY, moveSpeed, jumpSpeed, minYRot, maxYRot, minXRot, maxXRot, sprintMultiplier;
    public Rigidbody rb;
    public Transform theCam;
    public Light flashLight;
    public bool flashLightOn = false;

    private void Start()
    {
        Cursor.visible = false;
        FlashLightSet();
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        Vector3 moveDir = Vector3.zero;
        bool shouldMove = false;

        if (Input.GetKey(forward))
        {
            moveDir += transform.forward;
            shouldMove = true;
        }

        if (Input.GetKey(backward))
        {
            moveDir -= transform.forward;
            shouldMove = true;
        }

        if (Input.GetKey(left))
        {
            moveDir -= transform.right;
            shouldMove = true;
        }

        if (Input.GetKey(right))
        {
            moveDir += transform.right;
            shouldMove = true;
        }

        if (Input.GetKeyDown(sprint))
        {
            moveSpeed *= sprintMultiplier;
        }

        if (Input.GetKeyUp(sprint))
        {
            moveSpeed /= sprintMultiplier;
        }

        if (Input.GetKeyUp(flashLightSwitch))
        {
            ToggleFlashLight();
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

    private void ToggleFlashLight()
    {
        flashLightOn = !flashLightOn;
        FlashLightSet();
    }

    private void FlashLightSet()
    {
        flashLight.enabled = flashLightOn;
        if (flashLightOn)
        {
            flashLight.transform.Translate(Vector3.up);
        }
        else
        {
            flashLight.transform.Translate(Vector3.down);
        }
    }

    private void Move(Vector3 moveVector)
    {
        rb.velocity = moveVector * moveSpeed;
    }

    private void StopMoving()
    {
        rb.velocity = rb.velocity - rb.velocity * Time.deltaTime * moveSpeed;
    }

    private void FixedUpdate()
    {
        float rx = Input.GetAxis("Mouse Y") * mouseSpeedX;
        float ry = Input.GetAxis("Mouse X") * mouseSpeedY;

        theCam.Rotate(new Vector3(-rx, 0, 0) * Time.fixedDeltaTime);
        transform.Rotate(new Vector3(0, ry, 0) * Time.fixedDeltaTime);
    }
}
