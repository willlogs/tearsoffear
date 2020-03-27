using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public MultiplayerSystem.action a;
    public MultiplayerSystem.strAction sa;
    public bool isStr;
    public int input;
    public string sinput;

    public Action(int input, MultiplayerSystem.action a)
    {
        isStr = false;
        this.a = a;
        this.input = input;
    }

    public Action(string input, MultiplayerSystem.strAction a)
    {
        isStr = true;
        this.sa = a;
        this.sinput = input;
    }
}
