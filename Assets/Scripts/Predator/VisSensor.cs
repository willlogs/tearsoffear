using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisSensor : MonoBehaviour
{
    public static bool isVis = false;

    private void OnBecameVisible()
    {
        print("vis");
        isVis = true;
    }

    private void OnBecameInvisible()
    {
        print("invis");
        isVis = false;
    }
}
