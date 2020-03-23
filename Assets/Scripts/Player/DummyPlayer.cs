using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public Light flashLight;

    public void ToggleFlashLight()
    {
        flashLight.enabled = !flashLight.enabled;
    }
}
