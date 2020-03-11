using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static int count;

    private void Start()
    {
        count++;
    }

    public void GetCollected()
    {
        count--;
    }
}
