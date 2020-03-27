using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DuplicateTerminator : MonoBehaviour
{
    public static List<Interactive> interactives = new List<Interactive>();

    private void Awake()
    {
        interactives = new List<Interactive>();
        foreach (Interactive i in FindObjectsOfType<Interactive>())
        {
            interactives.Add(i);
        }
    }

    public static void Terminate()
    {
        int x = 0;
        foreach(Interactive i in interactives)
        {
            i.name = "Interactive(" + x + ")";
            x++;
        }
    }
}
