using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public Light flashLight;
    public Color flDefaultColor;

    bool uvOn = false;

    private void Start()
    {
        flDefaultColor = flashLight.color;
    }

    public void ToggleFlashLight()
    {
        flashLight.enabled = !flashLight.enabled;
    }

    public void UVToggle()
    {
        uvOn = !uvOn;

        if (uvOn)
        {
            flashLight.color = Color.cyan;
        }
        else
        {
            flashLight.color = flDefaultColor;
        }
    }
}
