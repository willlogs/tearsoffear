using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAnimations : MonoBehaviour
{
    public Animator animator;
    public bool walking = false;

    public void Walk()
    {
        if(walking == false)
        {
            walking = true;
            animator.SetBool("walking", true);
        }
    }

    public void Idle()
    {
        if (walking == true)
        {
            walking = false;
            animator.SetBool("walking", false);
        }
    }
}
