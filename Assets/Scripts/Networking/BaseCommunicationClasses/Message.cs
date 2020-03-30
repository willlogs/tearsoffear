using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPMessage
{
    public string mssg;
    public string hash;

    public bool timerOn = false;
    public float time = 0;

    public TCPMessage(string mssg, string hash)
    {
        this.mssg = mssg;
        this.hash = hash;
    }
}
