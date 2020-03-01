using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public List<Light> lights = new List<Light>();
    public bool turnOffLights = true;

    private void Start()
    {
        foreach(Light l in GetComponentsInChildren<Light>())
        {
            lights.Add(l);
            l.enabled = false;
        }
    }
}
