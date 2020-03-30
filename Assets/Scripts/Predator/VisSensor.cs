using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisSensor : MonoBehaviour
{
    public static bool isVis = false;

    public Transform player;

    private void Start()
    {
        player = FindObjectOfType<PlayerControl>().transform;
    }

    private void Update()
    {
        Vector3 diff = player.position - transform.position;
        diff = diff.normalized;
        RaycastHit hit;

        if(Physics.Raycast(transform.position, diff, out hit))
        {
            if(hit.transform == player)
            {
                isVis = true;
            }
            else
            {
                isVis = false;
            }
        }
    }

    //private void OnBecameVisible()
    //{
    //    print("vis");
    //    isVis = true;
    //}

    //private void OnBecameInvisible()
    //{
    //    print("invis");
    //    isVis = false;
    //}
}
