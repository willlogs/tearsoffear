using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    public bool predInCone = false;
    public Transform rayOrigin;
    public bool visible = false;

    Transform predator;

    private void Update()
    {
        if (predInCone)
        {
            RaycastHit[] hits;

            hits = Physics.RaycastAll(rayOrigin.position, (predator.position - rayOrigin.position), 30);

            bool hasHit = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == predator.gameObject)
                {
                    hasHit = true;
                    break;
                }
                else
                {
                    if (hit.collider.gameObject.layer == 9) continue;
                    else break;
                }
            }

            if (hasHit && !visible)
            {
                visible = true;
                BecameVisible();
            }

            if (!hasHit)
            {
                visible = false;
                BecameInvisible();
            }
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
            visible = false;
            predInCone = false;
            BecameInvisible();
        }
    }

    private void BecameVisible()
    {
        MultiplayerSystem.instance.SendVisPacket(true);
    }

    private void BecameInvisible()
    {
        MultiplayerSystem.instance.SendVisPacket(false);
    }
}
