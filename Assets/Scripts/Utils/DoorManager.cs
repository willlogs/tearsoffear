using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public List<Door> doors = new List<Door>();

    public static DoorManager instance;

    private void Start()
    {
        instance = this;

        int i = 0;
        foreach(Door d in FindObjectsOfType<Door>())
        {
            doors.Add(d);
            d.index = i++;
        }
    }

    public void Toggle(int index)
    {
        doors[index].ToggleState();
    }

    public void Toggled(int index)
    {
        MultiplayerSystem.instance.SendDoorTogglePacket(index);
    }
}
