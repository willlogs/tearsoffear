using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName ="SO/Dialouge")]
public class DialogueSO : ScriptableObject
{
    [SerializeField]
    public List<Message> dialogue = new List<Message>();

    private int index;

    private void OnEnable()
    {
        index = 0;
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    public bool HasNext()
    {
        return index < dialogue.Count;
    }

    public Message NextMessage()
    {
        if (index < dialogue.Count)
            return dialogue[index++];
        else return null;
    }
}
