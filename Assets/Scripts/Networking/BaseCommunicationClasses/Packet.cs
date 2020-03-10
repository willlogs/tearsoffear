using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Packet
{
    public TransformData td;

    public bool tdSet = false, isPred = false;

    public PacketType type;

    public int index; // index of the dummy in the scene
    public int targetIndex;

    public Packet(int index, TransformData td = null, bool tdSet = false, PacketType type = PacketType.TRANSFORMDATA, bool ip = false, int sIndex = 0)
    {
        this.isPred = ip;
        this.index = index;
        this.td = td;
        this.tdSet = tdSet;
        this.type = type;
        this.targetIndex = sIndex;
    }
}
