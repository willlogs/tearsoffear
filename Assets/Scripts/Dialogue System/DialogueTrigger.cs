using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string sensitiveTag = "Player";
    public DialogueSystem dialogueSystem;
    public DialogueSO dialogue;

    private bool triggered = false;

    public void SetDialogueSystem(DialogueSystem ds)
    {
        dialogueSystem = ds;
    }

    public void SayNext()
    {
        if (dialogue.HasNext())
        {
            Message next = dialogue.NextMessage();
            dialogueSystem.Say(next, SayNext);
        }
        else
        {
            print("dialogue over");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered && other.tag == sensitiveTag)
        {
            SayNext();
            triggered = true;
        }
    }
}
