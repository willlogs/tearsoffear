using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void TimerCallBack();

    private TimerCallBack cb;
    private float time, duration;
    private bool isSet, destroyAfterDone;

    public Timer(float duration, TimerCallBack cb, bool destroyAfterDone = true, bool startIt = true)
    {
        SetTimer(duration, cb, destroyAfterDone, startIt);
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

    public void SetTimer(float duration, TimerCallBack cb, bool destroyAfterDone = true, bool startIt = true)
    {
        this.duration = duration;
        this.cb = cb;
        this.isSet = startIt;
        this.destroyAfterDone = destroyAfterDone;
    }

    public void Reset()
    {
        this.isSet = true;
    }
}
