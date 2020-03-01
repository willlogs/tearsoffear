using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData
{
    public Quaternion rotation;
    public Vector3 position;
    public bool isSet = false;

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
