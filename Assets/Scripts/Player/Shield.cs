using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    public delegate void CB();

    public event CB OnDeactivated;

    public Slider chargeSlider;
    public float rechargeTime, effectTime;
    public bool recharging;
    public GameObject shieldEffect;

    float charge = 1, time;
    bool waiting = false;

    private void Start()
    {
        Slider s = FindObjectOfType<Slider>();
        if(s.tag == "ChargingSystem")
        {
            chargeSlider = s;
        }
    }

    private void Update()
    {
        if (waiting)
        {
            time += Time.deltaTime;
            if(time >= effectTime)
            {
                time = 0;
                OnDeactivated?.Invoke();
                shieldEffect.SetActive(false);
                waiting = false;
            }
        }
        else
        {
            charge += Time.deltaTime / rechargeTime;

            if (charge >= 1)
            {
                charge = 1;
                recharging = false;
            }
        }

        chargeSlider.value = charge;
    }

    public bool Boom()
    {
        if (!recharging)
        {
            recharging = true;
            waiting = true;
            shieldEffect.SetActive(true);

            charge = 0;

            return true;
        }

        return false;
    }
}
