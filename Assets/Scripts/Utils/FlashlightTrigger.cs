using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    public bool predInCone = false;
    public Transform rayOrigin;
    public bool visible = false;

    Transform predator;
    bool isVis = false;

    private void Update()
    {
        if(visible && predInCone)
        {
            BecameVisible();
        }
        else
        {
            BecameInvisible();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Predator")
        {
            predInCone = true;
            predator = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Predator" && predInCone)
        {
            predInCone = false;
            BecameInvisible();
        }
    }

    private void BecameVisible()
    {
        if (!isVis)
        {
            isVis = true;
            MultiplayerSystem.instance.SendVisPacket(true);
        }
    }

    private void BecameInvisible()
    {
        if (isVis)
        {
            isVis = false;
            MultiplayerSystem.instance.SendVisPacket(false);
        }
    }
}
