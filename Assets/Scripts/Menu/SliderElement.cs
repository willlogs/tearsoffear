using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderElement : MonoBehaviour
{
    public string name_;

    private void Start()
    {
        UISingleton<Slider>.dic.Add(name_, GetComponent<Slider>());
    }
}