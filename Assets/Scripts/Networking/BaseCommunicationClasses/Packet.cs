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
    public string targetString;

    public string hash;

    public Packet(int index, string hash,TransformData td = null, bool tdSet = false, PacketType type = PacketType.TRANSFORMDATA, bool ip = false, int sIndex = 0, string targetString = "")
    {
        this.targetString = targetString;
        this.hash = hash;
        this.isPred = ip;
        this.index = index;
        this.td = td;
        this.tdSet = tdSet;
        this.type = type;
        this.targetIndex = sIndex;
    }
}
