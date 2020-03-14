using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    public bool predInCone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Predator")
        {
            RaycastHit[] hits;

            hits = Physics.RaycastAll(transform.parent.position, (other.transform.position - transform.parent.position), 30);

            bool hasHit = false;
            foreach (RaycastHit hit in hits)
            {
                if(hit.collider == other)
                {
                    hasHit = true;
                }
                else
                {
                    if (hit.collider.gameObject.layer == 9) continue;
                    else break;
                }
            }

            if (!predInCone && hasHit) 
            {
                predInCone = hasHit;
                BecameVisible();
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Predator" && predInCone)
        {
            predInCone = false;
            BecameUnvisible();
        }
    }

    private void BecameVisible()
    {
        MultiplayerSystem.instance.SendVisPacket(true);
    }

    private void BecameUnvisible()
    {
        MultiplayerSystem.instance.SendVisPacket(false);
    }
}
