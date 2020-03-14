using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictimSystem : MonoBehaviour
{
    public float armsReach = 5;
    public bool hasTarget = false;
    public int targetIndex = 0;
    public Camera cam;

    public Image handIcon;
    public Text plusSign;

    public Sprite scareIcon, handSprite;

    Vector3 mousePos;

    private void Start()
    {
        cam = GetComponent<Camera>();

        handIcon = FindObjectOfType<HandIcon>().GetComponent<Image>();
        plusSign = FindObjectOfType<PlusSign>().GetComponent<Text>();
    }

    private void Update()
    {
        ShootRay();
    }

    private void ShootRay()
    {
        mousePos = transform.position + transform.forward * armsReach;

        RaycastHit[] hit;
        Ray ray = new Ray(transform.position, transform.forward);

        hit = Physics.RaycastAll(ray, armsReach);

        if (hit.Length > 0)
        {
            bool hasHit = false;
            int i = 0;
            for (i = 0; i < hit.Length; i++)
            {
                string tagName = hit[i].collider.tag;
                if (tagName == "Dummy")
                {
                    hasHit = true;
                    break;
                }
                else
                {
                    if (tagName != "Predator") break;
                }
            }

            if (hasHit)
            {
                plusSign.enabled = false;
                handIcon.enabled = true;

                handIcon.sprite = scareIcon;

                hasTarget = true;

                targetIndex = hit[i].collider.GetComponent<Dummy>().index;
            }
            else
            {
                plusSign.enabled = true;
                handIcon.enabled = false;
                hasTarget = false;
            }
        }
        else
        {
            plusSign.enabled = true;
            handIcon.enabled = false;
            hasTarget = false;
        }
    }
}
