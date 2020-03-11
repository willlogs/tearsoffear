using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    #region Public Variables
    public static MultiplayerSystem instance;

    // Prefabs and SpawnPoses
    public GameObject playerPrefab, dummyPrefab, predatorPref, dummyPredatorPref;
    public PositionKeeper spawnPositions;
    public Transform predSpawnPos;

    // Connection
    public TCPConnection con;

    public MultiplayerTools tools;

    // Options
    public bool isCli, printLastPacket = true;
    public int conIndex = 0; // conIndex + 1 is the index of the dummy in the scene (if isCli is on) - index of srv is 0
    #endregion

    #region Private Variables
    // Private things
    delegate void action();
    List<action> actions = new List<action>();

    // connectino management
    bool connected = false, isPred = false;
    int connections = 0;
    TransformData td;

    // For printing last packet
    string lastPacket;
    bool newPacket = false;
    #endregion

    #region public methods
    public void Initialize()
    {
        // TODO: turn this to a method in tools
        tools = gameObject.AddComponent<MultiplayerTools>();
        tools.playerPrefab = playerPrefab;
        tools.dummyPrefab = dummyPrefab;
        tools.predatorPref = predatorPref;
        tools.dummyPredatorPref = dummyPredatorPref;
        tools.spawnPositions = spawnPositions;
        tools.predSpawnPos = predSpawnPos;

        InitializeSystem();
        instance = this;
    }

    public void SendScarePacket(int targetIndex)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, sIndex: targetIndex, type: PacketType.SCARE);

        con.SendMessage_(JsonUtility.ToJson(p));
    }
    #endregion

    private void InitializeSystem()
    {
        con.BindEventHandler(OnRecieveData, 0);

        if (isCli)
        {
            con.StartIt();
            con.BindEventHandler(OnConnectionStablishedCli, 1);
        }
        else
        {
            con.BindEventHandler(OnConnectionStablishedSrv, 1);
            tools.AddMonster();
            StartServer();
        }
    }

    private void StartServer()
    {
        // call srvr.StartListening() as async function
        TCPServer.StartListeningAsync startListeningAsync = new TCPServer.StartListeningAsync(con.StartIt);
        startListeningAsync.BeginInvoke(null, null);
    }

    private void Update()
    {
        if(printLastPacket)
            PrintLastPacket();

        DoActions();
        if (connected)
        {
            SendTD();
            tools.UpdateTransforms(isCli, conIndex);
        }
    }

    private void DoActions()
    {
        if (actions.Count > 0)
        {
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                actions[i]();
                actions.RemoveAt(i);
            }
        }
    }

    private void SendTD()
    {
        int index = !isCli ? 0 : conIndex + 1;
        GameObject reference = tools.dummies[index];
        if (td == null || td.rotation != reference.transform.rotation || td.position != reference.transform.position)
        {
            td = new TransformData(reference.transform.rotation, reference.transform.position);
            td.isSet = true;
            Packet p = new Packet(index, td: td, tdSet: true);

            if (isCli)
            {
                con.SendMessage_(JsonUtility.ToJson(p));
            }
            else
            {
                con.BroadCastMessage(JsonUtility.ToJson(p));
            }
        }
    }

    private void SetTD(Packet p)
    {
        try
        {
            TransformData td = p.td;
            td.isSet = true;
            tools.transformData[p.index] = td;
        }
        catch(Exception e)
        {
            print("err seting td: " + e.ToString());
        }
    }

    private void OnRecieveData(string data)
    {
        try
        {
            con.BroadCastMessage(data);

            lastPacket = data;
            newPacket = true;

            Packet p = JsonUtility.FromJson<Packet>(data);

            switch (p.type)
            {
                case PacketType.NEWCLI:
                    if (p.isPred)
                    {
                        actions.Add(tools.AddDummyMonster);
                    }
                    else
                    {
                        actions.Add(tools.AddDummy);
                    }
                    break;

                case PacketType.TRANSFORMDATA:
                    if (p.tdSet)
                    {
                        SetTD(p);
                    }
                    break;

                case PacketType.SCARE:
                    print("scare message " + p.targetIndex);
                    if (p.targetIndex == conIndex + 1)
                    {
                        actions.Add(GetScared);
                    }
                    break;

                // TODO: fix when pred is chosen randomly
                case PacketType.HIT:
                    if (!isCli)
                    {
                        tools.dummies[0].GetComponent<PredatorControl>().Die();
                    }
                    break;
            }
        }
        catch(Exception e)
        {
            print("parsing err: " + e);
        }
    }

    private void OnConnectionStablishedCli(string data)
    {
        lastPacket = data;
        newPacket = true;

        print("con stab");
        print(data);
        conIndex = Convert.ToInt32(data);
        actions.Add(InstantiateCli);
        connected = true;
    }

    private void OnConnectionStablishedSrv(string data)
    {
        lastPacket = data;
        newPacket = true;

        conIndex = Convert.ToInt32(data);
        if (conIndex == 0)
        {
            actions.Add(InstantiateSrv);
        }
        else
        {
            actions.Add(tools.AddDummy);
            Packet p = new Packet(conIndex, type: PacketType.NEWCLI);
            con.BroadCastMessage(JsonUtility.ToJson(p), conIndex);
        }
        connected = true;
    }

    private void SendMessageTo(string mssg, int i = 0)
    {
        con.SendMessage_(mssg, i);
    }

    private void InstantiateSrv()
    {
        tools.AddDummy();
    }

    private void InstantiateCli()
    {
        for (int i = 0; i <= conIndex; i++)
        {
            if (i == 0)
            {
                tools.AddDummyMonster();
            }
            else
            {
                GameObject temp = Instantiate(dummyPrefab, spawnPositions.poses[i].position, Quaternion.identity);
                tools.dummies.Add(temp);
                temp.AddComponent<Dummy>().index = tools.dummies.Count - 1;
                tools.transformData.Add(new TransformData());
            }
        }

        tools.AddPlayer();
    }
    
    private void PrintLastPacket()
    {
        try
        {
            if (newPacket)
            {
                OnScreenConsole.Instance.Print(lastPacket);
                newPacket = false;
            }
        }
        catch { }
    }

    private void GetScared()
    {
        print("getting scared");
        PlayerControl pc = tools.dummies[conIndex + 1].GetComponent<PlayerControl>();

        if (pc.shielded)
        {
            Packet p = new Packet(conIndex + 1, type: PacketType.HIT, sIndex: 0);
            SendMessageTo(JsonUtility.ToJson(p));
        }
        else
        {
            pc.GetScared();
        }
    }
}
