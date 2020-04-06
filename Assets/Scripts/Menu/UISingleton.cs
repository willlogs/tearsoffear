using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISingleton<T> : MonoBehaviour
{
    public static Dictionary<string, T> dic = new Dictionary<string, T>();

    public static T GetByName(string name)
    {
        try
        {
            return dic[name];
        }
        catch
        {
            Debug.LogWarning("Couldn't find the ui you wanted: " + name);
            return default(T);
        }
    }
}
