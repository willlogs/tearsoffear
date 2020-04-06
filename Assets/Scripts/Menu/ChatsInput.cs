using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatsInput : MonoBehaviour
{
    bool active = false;
    InputField input;

    private void Start()
    {
        input = UISingleton<InputField>.GetByName("ChatsInput");
        input.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            active = !active;

            if (active)
            {
                input.enabled = true;
                input.Select();
            }
            else
            {
                OnScreenChat.Print("YOU: " + input.text);
                MultiplayerSystem.instance.SendChatMessage(input.text);
                input.text = "";
                input.enabled = false;
            }
        }
    }
}