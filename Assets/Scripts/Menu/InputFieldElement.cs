using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldElement : MonoBehaviour
{
    public string name_;

    private void Start()
    {
        UISingleton<InputField>.dic.Add(name_, GetComponent<InputField>());
    }
}
