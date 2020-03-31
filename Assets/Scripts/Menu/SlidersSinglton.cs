using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersSinglton : MonoBehaviour
{
    public static Dictionary<string, SlidersSinglton> sliders = new Dictionary<string, SlidersSinglton>();

    public string name;

    public Slider slider;

    public static SlidersSinglton GetByName(string name)
    {

        try
        {
            return sliders[name];
        }
        catch
        {
            Debug.LogWarning("Couldn't find the slider you wanted: " + name);
            return null;
        }
    }

    private void Start()
    {
        sliders.Add(name, this);
    }
}
