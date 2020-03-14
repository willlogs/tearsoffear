using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPredator : MonoBehaviour
{
    public bool visible = false;

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
