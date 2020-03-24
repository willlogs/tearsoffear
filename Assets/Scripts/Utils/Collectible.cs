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
        OnScreenConsole.Instance.Print(count + " collectibles to collect find them to win!");
        OnScreenConsole.Instance.Print("If you cast flashlight on the ghost it can't move! you can also hear her when she's near you!");
    }

    public static void GetCollected(int index)
    {
        count--;
        Destroy(list[index].gameObject);

        if(count > 0)
            OnScreenConsole.Instance.Print("collectible collected! " + count + " more left");
        else
            OnScreenConsole.Instance.Print("People won!!! No More Collectibles!");
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
