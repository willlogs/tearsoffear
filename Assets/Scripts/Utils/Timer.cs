using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void TimerCallBack();

    private TimerCallBack cb;
    private float time, duration;
    private bool isSet, destroyAfterDone;

    public Timer(float duration, TimerCallBack cb, bool destroyAfterDone = true)
    {
        SetTimer(duration, cb, destroyAfterDone);
    }

    public Timer() { }

    private void Update()
    {
        if (isSet)
        {
            time += Time.deltaTime;
            if (time > duration)
            {
                time = 0;
                isSet = false;
                cb();
                if (destroyAfterDone)
                {
                    Destroy(this);
                }
            }
        }
    }

    public void SetTimer(float duration, TimerCallBack cb, bool destroyAfterDone = true)
    {
        this.duration = duration;
        this.cb = cb;
        this.isSet = true;
        this.destroyAfterDone = destroyAfterDone;
    }
}
