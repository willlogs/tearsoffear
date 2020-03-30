using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packer
{
    private static char seperator = '%';

    public static string Pack(TCPMessage[] mssgs)
    {
        string s = "";
        for (int i = 0; i < mssgs.Length; i++)
        {
            s += mssgs[i].mssg;
            
            if(i != mssgs.Length - 1)
            {
                s += seperator;
            }
        }

        return s;
    }

    public static string Stringify(string[] packets)
    {
        return String.Join(seperator + "", packets);
    }

    public static string[] Parse(string s)
    {
        return s.Split(seperator);
    }
}
