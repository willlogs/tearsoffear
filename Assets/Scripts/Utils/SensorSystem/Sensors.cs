using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour
{
    public Dictionary<string, Sensor> sensorsDic = new Dictionary<string, Sensor>();
    public Sensor[] sensors;

    private void Awake()
    {
        foreach(Sensor s in sensors)
        {
            sensorsDic.Add(s.gameObject.name, s);
        }
    }
}
