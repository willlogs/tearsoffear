using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    protected int state;

    protected bool mustSetState;
    protected int nextState;

    public int index;

    private void Update()
    {
        if(mustSetState)
        {
            ApplyState();
            mustSetState = false;
        }
    }

    protected abstract void ApplyState();

    public abstract void Interact();
    public abstract void GhostInteract();
}
