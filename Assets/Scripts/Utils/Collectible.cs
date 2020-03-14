using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : Interactive
{
    public static int count;
    public static List<Collectible> list = new List<Collectible>();

    public int index;

    private void Start()
    {
        list.Add(this);
        index = list.Count - 1;
        count++;
    }

    public static void GetCollected(int index)
    {
        count--;
        Destroy(list[index].gameObject);
    }

    public override void Interact()
    {
        MultiplayerSystem.instance.SendCollectPacket(index);
        GetCollected(index);
    }

    public override void GhostInteract()
    {

    }
}
