using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public delegate void OnTriggerEvent();

    public event OnTriggerEvent OnTriggered, OnUntriggered;
    public bool triggered = false;

    int collides;

    private void OnTriggerEnter(Collider other)
    {
        collides++;
        CheckTriggered();
    }

    private void OnTriggerExit(Collider other)
    {
        collides--;
        CheckTriggered();
    }

    private void CheckTriggered()
    {
        if (collides > 0)
        {
            triggered = true;
            OnTriggered?.Invoke();
        }
        else 
        { 
            triggered = false;
            OnUntriggered?.Invoke();
        }
    }
}
