using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenConsole : MonoBehaviour
{
    public static OnScreenConsole instance;

    public Text consoleText;
    public int lines = 20;

    private void Start()
    {
        instance = this;
    }

    public void Print(string msg)
    {
        consoleText.text += "\n" + msg;
        string[] splitted = consoleText.text.Split('\n');
        if (splitted.Length > lines)
        {
            consoleText.text = string.Join("\n", splitted, 1, splitted.Length - 1);
        }
    }
}
