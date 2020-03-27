using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static List<Door> doors = new List<Door>();

    public static DoorManager instance;

    private void Start()
    {
        instance = this;

        int i = 0;
        foreach(Door d in FindObjectsOfType<Door>())
        {
            doors.Add(d);
            d.indexx = i++;
        }
    }

    public static void Toggle(string name)
    {
        foreach(Door d in doors)
        {
            if(d.name == name)
            {
                d.ToggleState();
            }
        }
    }

    public void Toggled(string name)
    {
        MultiplayerSystem.instance.SendDoorTogglePacket(name);
    }
}
