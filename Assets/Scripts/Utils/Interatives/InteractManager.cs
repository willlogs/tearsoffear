using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public List<Interactive> list = new List<Interactive>();
    public static InteractManager instance;

    private void Start()
    {
        instance = this;

        int x = 0;
        foreach(Interactive i in FindObjectsOfType<Interactive>())
        {
            list.Add(i);
            i.index = x++;
        }
    }
}
