using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Controller
{
    public KeyCode jump, sprint, flashLightSwitch, activateShield;
    public float sprintMultiplier, jumpSpeed;
    public Light flashLight;
    public bool flashLightOn = false, shielded;
    public DummyAnimations animations;
    public ScareEffect se;
    public Shield shield;

    public void GetScared()
    {
        se.Scare();
        Die();
    }

    protected override void Start()
    {
        FlashLightSet();
        animations = GetComponent<DummyAnimations>();
        shield.OnDeactivated += DeativateShield;
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

        if (Input.GetKeyDown(activateShield))
        {
            shielded = shield.Boom();
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

    private void DeativateShield()
    {
        shielded = false;
    }
}
