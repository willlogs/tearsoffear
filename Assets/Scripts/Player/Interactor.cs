using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    public float armsReach;
    public Sprite handIcon;

    Vector3 mousePos;
    Image iconPlace;
    Text plusSign;

    Interactive interactive;
    bool hasTarget;

    public void Interact()
    {
        if (hasTarget)
        {
            interactive.Interact();
        }
    }

    private void Start()
    {
        iconPlace = FindObjectOfType<HandIcon>().GetComponent<Image>();
        plusSign = FindObjectOfType<PlusSign>().GetComponent<Text>();
    }

    private void Update()
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
                if (tagName == "Interactable")
                {
                    hasHit = true;
                    interactive = hit[i].collider.GetComponent<Interactive>();
                    hasTarget = true;
                    break;
                }
                else
                {
                    if (tagName != "Player" && hit[i].collider.gameObject.layer != 9) break;
                }
            }

            if (hasHit)
            {
                SetHandIcon();
            }
            else
            {
                SetPlusSign();
            }
        }
        else
        {
            SetPlusSign();
        }
    }

    private void SetHandIcon()
    {
        iconPlace.enabled = true;
        iconPlace.sprite = handIcon;
        plusSign.enabled = false;
    }

    private void SetPlusSign()
    {
        plusSign.enabled = true;
        iconPlace.enabled = false;
    }
}
