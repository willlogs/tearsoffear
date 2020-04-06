using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextElement : MonoBehaviour
{
    public string name_;

    private void Start()
    {
        UISingleton<Text>.dic.Add(name_, GetComponent<Text>());
    }
}
