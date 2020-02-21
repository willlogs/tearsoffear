using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text text;
    public float betweenLetters;

    public delegate void CallBack();
    public CallBack onDoneCB;

    private Timer timer;
    private Message curMessage;
    private int messageIndex;
    private bool isRunning;

    private void Start()
    {
        timer = gameObject.AddComponent<Timer>();
        text.enabled = false;
        FindObjectOfType<DialogueTrigger>().SetDialogueSystem(this);
    }

    public void Say(Message message, CallBack cb_)
    {
        text.enabled = true;
        onDoneCB = cb_;
        curMessage = message;
        messageIndex = 0;
        isRunning = true;
        ShowNextLetter();
    }
    
    public void ShowNextLetter()
    {
        if (isRunning && messageIndex < curMessage.message.Length)
        {
            timer.SetTimer(betweenLetters, ShowNextLetter, false);
            text.text = curMessage.actorName + ": " + curMessage.message.Substring(0, ++messageIndex);
        }
        else
        {
            timer.SetTimer(curMessage.showForSecs, OnMessageEnded, false);
        }
    }

    public void Skip()
    {
        // if isRunning => show the whole message

        // else => next Message

    }

    public void OnMessageEnded()
    {
        text.enabled = false;
        onDoneCB();
    }
}
