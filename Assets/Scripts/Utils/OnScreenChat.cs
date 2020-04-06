using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenChat : MonoBehaviour
{
    static private Text chatText;
    static int lines = 11;

    private void Start()
    {
        chatText = UISingleton<Text>.GetByName("ChatsText");
    }

    public static void Print(string mssg)
    {
        chatText.text += "\n" + mssg;
        string[] splitted = chatText.text.Split('\n');
        if (splitted.Length > lines)
        {
            chatText.text = string.Join("\n", splitted, 1, splitted.Length - 1);
        }
    }
}
