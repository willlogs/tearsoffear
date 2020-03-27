using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class TransformData
{
    public Quaternion rotation;
    public Vector3 position;
    public bool isSet = false;

    [System.NonSerialized]
    public bool hasTween;
    [System.NonSerialized]
    public Tweener tween;

    public TransformData(Quaternion r, Vector3 p)
    {
        UpdateValues(r, p);
    }

    public TransformData()
    {
        UpdateValues(Quaternion.identity, Vector3.zero);
    }

    private void UpdateValues(Quaternion r, Vector3 p)
    {
        rotation = r;
        position = p;
    }
}
