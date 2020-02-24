using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenConsole : MonoBehaviour
{
    public static OnScreenConsole Instance;

    public Text consoleText;

    private void Start()
    {
        Instance = this;
    }

    public void Print(string msg)
    {
        consoleText.text += "\n" + msg;
        string[] splitted = consoleText.text.Split('\n');
        if (splitted.Length > 15)
        {
            consoleText.text = string.Join("\n", splitted, 1, splitted.Length - 1);
        }
    }
}
