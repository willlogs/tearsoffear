using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Controller
{
    public KeyCode jump, sprint, flashLightSwitch;
    public float sprintMultiplier, jumpSpeed;
    public Light flashLight;
    public bool flashLightOn = false;
    public DummyAnimations animations;
    public ScareEffect se;

    public void GetScared()
    {
        se.Scare();
    }

    protected override void Start()
    {
        FlashLightSet();
        animations = GetComponent<DummyAnimations>();
    }

    protected override void CheckInput()
    {
        if (Input.GetKeyUp(flashLightSwitch))
        {
            ToggleFlashLight();
        }

        if (Input.GetKeyDown(sprint))
        {
            moveSpeed *= sprintMultiplier;
        }

        if (Input.GetKeyUp(sprint))
        {
            moveSpeed /= sprintMultiplier;
        }

        base.CheckInput();

        if (shouldMove)
        {
            if (!animations.walking) animations.Walk();
        }
        else
        {
            if (animations.walking) animations.Idle();
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
}
