using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragRigidBody : MonoBehaviour
{
    public float armLength = 2, armSpeed;
    public Sprite handOpen, handGrab;
    public Text plusSign;
    public Image handIcon;    

    Rigidbody targetRb;
    Collider targetCollider;
    bool hasTarget = false, grabbed = false;
    Vector3 diffFromMouse, mousePos;

    private void Update()
    {
        mousePos = transform.position + transform.forward * armLength;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, armLength))
        {
            if(targetRb && hit.collider.gameObject != targetRb.gameObject)
                mousePos = hit.point;

            if (!grabbed)
            {
                hit.collider.TryGetComponent<Rigidbody>(out targetRb);
                if (targetRb)
                {
                    hasTarget = true;
                    handIcon.enabled = true;
                }
                else
                {
                    hasTarget = false;
                    handIcon.enabled = false;
                }
            }
        }
        else
        {
            if (!grabbed)
            {
                hasTarget = false;
                handIcon.enabled = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && hasTarget)
        {
            Grab();
        }

        if (Input.GetMouseButtonUp(0) && grabbed)
        {
            Drop();
            targetRb = null;
            hasTarget = false;
        }

        if(hasTarget && grabbed)
        {
            targetRb.velocity = (mousePos - targetRb.transform.position) * armSpeed;
        }
    }

    private void Grab()
    {
        handIcon.sprite = handGrab;
        targetRb.useGravity = false;
        grabbed = true;
        diffFromMouse = mousePos - targetRb.transform.position;
    }

    private void Drop()
    {
        handIcon.sprite = handOpen;
        targetRb.useGravity = true;
        grabbed = false;
    }
}
