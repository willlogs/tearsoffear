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
    public Interactor interactor;

    // UV light
    public FlashlightTrigger fl;
    public float flashLightCharge = 1;
    public float dechargeIn = 5; // in minutes
    public Color defaultFLColor;
    public bool uvOn = false;
    public SlidersSinglton flchargeSlider;
    public AudioSource uvSound;

    public void GetScared()
    {
        se.Scare();
        Die();
    }

    protected override void Start()
    {
        FlashLightSet();
        flchargeSlider = SlidersSinglton.GetByName("FL");

        defaultFLColor = flashLight.color;

        animations = GetComponent<DummyAnimations>();
        shield.OnDeactivated += DeativateShield;
    }

    protected override void Update()
    {
        base.Update();

        if (flashLightOn)
        {
            flashLightCharge -= uvOn? Time.deltaTime / 5 / 60 * 10: Time.deltaTime / 5 / 60;
            if (flashLightCharge <= 0)
            {
                flashLightCharge = 0;
                ToggleFlashLight();
            }
            flchargeSlider.slider.value = flashLightCharge;
        }
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

        if (Input.GetMouseButtonDown(0))
        {
            interactor.Interact();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (flashLightOn)
            {
                fl.visible = true;
                flashLight.color = Color.cyan;
                uvOn = true;
                uvSound.Play();
                MultiplayerSystem.instance.SendToggleUVPacket();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            fl.visible = false;
            flashLight.color = defaultFLColor;
            uvOn = false;
            uvSound.Stop();
            MultiplayerSystem.instance.SendToggleUVPacket();
        }
    }

    private void ToggleFlashLight()
    {
        if (flashLightOn || (!flashLightOn && flashLightCharge > 0))
        {
            flashLightOn = !flashLightOn;

            if(!flashLightOn && uvOn)
            {
                fl.visible = false;
                flashLight.color = defaultFLColor;
                uvOn = false;
                uvSound.Stop();
                MultiplayerSystem.instance.SendToggleUVPacket();
            }

            FlashLightSet();
            MultiplayerSystem.instance.SendFlashLightPacket();
        }
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
