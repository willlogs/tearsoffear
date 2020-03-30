using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : Interactive
{
    public static int count;
    public static List<Collectible> list = new List<Collectible>();

    public int indexx;

    private void Start()
    {
        list.Add(this);
        indexx = list.Count - 1;
        count++;
    }

    public static void GetCollected(string name)
    {
        for(int i = list.Count - 1; i >= 0; i--)
        {            
            if (list[i].name == name)
            {
                Destroy(list[i].gameObject);
                list.RemoveAt(i);
                count--;
            }
        }
    }

    public override void Interact()
    {
        MultiplayerSystem.instance.SendCollectPacket(gameObject.name);
        GetCollected(gameObject.name);
    }

    public override void GhostInteract()
    {

    }

    protected override void ApplyState()
    {

    }
}
