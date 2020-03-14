using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public MultiplayerSystem.action a;
    public int input;

    public Action(int input, MultiplayerSystem.action a)
    {
        this.a = a;
        this.input = input;
    }
}
