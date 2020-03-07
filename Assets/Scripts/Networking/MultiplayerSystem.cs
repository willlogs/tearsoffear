using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    public GameObject playerPrefab, dummyPrefab, predatorPref, dummyPredatorPref;
    public PositionKeeper spawnPositions;
    public Transform predSpawnPos;

    public List<GameObject> dummies = new List<GameObject>();
    public List<TransformData> transformData = new List<TransformData>();
    public TCPClient cli;
    public TCPServer srv;
    public bool isCli, isTest, printLastPacket = true;
    public int conIndex = 0; // conIndex + 1 is the index of the dummy in the scene (if isCli is on) - index of srv is 0

    delegate void action();
    List<action> actions = new List<action>();

    bool connected = false, isPred = false;
    int connections = 0;
    TransformData td;

    string lastPacket;
    bool newPacket = false;

    public void Initialize()
    {
        if (isTest)
        {
            cli.OnRecieveData += OnReceiveDataTEST;
            srv.OnRecieveData += OnReceiveDataTEST;

            cli.OnConnected += OnConnectionStablishedTEST;
            srv.OnConnected += OnConnectionStablishedTEST;

            StartServer();

            cli.StartConnection();
        }
        else
        {
            if (isCli)
            {
                cli.OnRecieveData += OnRecieveData;
                cli.OnConnected += OnConnectionStablishedCli;

                cli.StartConnection();
            }
            else
            {
                srv.OnRecieveData += OnRecieveData;
                srv.OnConnected += OnConnectionStablishedSrv;

                AddMonster();

                StartServer();
            }
        }
    }

    public void SendScarePacket(int targetIndex)
    {
        int index = !isCli ? 0 : conIndex + 1;
        Packet p = new Packet(index, sIndex: targetIndex, type: PacketType.SCARE);

        if (isCli)
        {
            // potential problem in server and passing the message to the proper one
            cli.SendMessage_(JsonUtility.ToJson(p));
        }
        else
        {
            OnScreenConsole.Instance.Print("sending scare packet " + targetIndex);
            srv.SendMessage_(JsonUtility.ToJson(p), targetIndex - 1);
        }
    }

    private void StartServer()
    {
        // call srvr.StartListening() as async function
        TCPServer.StartListeningAsync startListeningAsync = new TCPServer.StartListeningAsync(srv.StartListening);
        startListeningAsync.BeginInvoke(null, null);
    }

    private void Update()
    {
        PrintLastPacket();
        DoActions();
        if (!isTest && connected)
        {
            SendTD();
            UpdateTD();
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
        GameObject reference = dummies[index];
        if (td == null || td.rotation != reference.transform.rotation || td.position != reference.transform.position)
        {
            td = new TransformData(reference.transform.rotation, reference.transform.position);
            td.isSet = true;
            Packet p = new Packet(index, td: td, tdSet: true);

            if (isCli)
            {
                cli.SendMessage_(JsonUtility.ToJson(p));
            }
            else
            {
                srv.BroadCastMessage(JsonUtility.ToJson(p));
            }
        }
    }

    private void UpdateTD()
    {
        int index = !isCli ? 0 : conIndex + 1;
        int i = 0;
        foreach(TransformData td in transformData)
        {
            bool shouldSkip = (isCli && i == conIndex + 1) || (!isCli && i == 0);

            if (td.isSet)
            {
                if (!shouldSkip)
                {
                    dummies[i].transform.rotation = td.rotation;
                    dummies[i].transform.position = td.position;

                    // TODO: optimize this with a list of dummies
                    dummies[i].GetComponent<DummyAnimations>().sfx.Walk();
                }
                td.isSet = false;
            }            

            i++;
        }
    }

    private void SetTD(Packet p)
    {
        try
        {
            TransformData td = p.td;
            td.isSet = true;
            transformData[p.index] = td;
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
            if (!isCli && srv.connectionsState.Count > 1)
            {
                print("bc: " + data);
                srv.BroadCastMessage(data);
            }

            lastPacket = data;
            newPacket = true;

            Packet p = JsonUtility.FromJson<Packet>(data);

            switch (p.type)
            {
                case PacketType.NEWCLI:
                    if (p.isPred)
                    {
                        actions.Add(AddNewMonsterDummy);
                    }
                    else
                    {
                        actions.Add(AddNewDummy);
                    }
                    break;

                case PacketType.TRANSFORMDATA:
                    if (p.tdSet)
                    {
                        SetTD(p);
                    }
                    break;

                case PacketType.SCARE:
                    print("scare message " + p.scareIndex);
                    if (p.scareIndex == conIndex + 1)
                    {
                        actions.Add(GetScared);
                    }
                    break;
            }
        }
        catch(Exception e)
        {
            print("parsing err: " + e);
        }
    }

    private static void OnReceiveDataTEST(string data)
    {
        print(data);
    }

    private void OnConnectionStablishedCli(string data)
    {
        conIndex = Convert.ToInt32(data);
        actions.Add(InstantiateCli);
        connected = true;
    }

    private void OnConnectionStablishedSrv(string data)
    {
        conIndex = Convert.ToInt32(data);
        if (conIndex == 0)
        {
            actions.Add(InstantiateSrv);
        }
        else
        {
            actions.Add(AddNewDummy);
            Packet p = new Packet(conIndex, type: PacketType.NEWCLI);
            srv.BroadCastMessage(JsonUtility.ToJson(p), conIndex);
        }
        connected = true;
    }

    private void OnConnectionStablishedTEST(string data)
    {
        connections++;
        print("new connection stablished : " + connections);
        if (connections == 2)
        {
            srv.SendMessage_("Hi!");
            cli.SendMessage_("Hi from cli!");
        }
    }

    private void InstantiateSrv()
    {
        dummies.Add(Instantiate(dummyPrefab, spawnPositions.poses[0].position, Quaternion.identity));
        dummies[dummies.Count - 1].AddComponent<Dummy>().index = dummies.Count - 1;
        transformData.Add(new TransformData());
    }

    private void InstantiateCli()
    {
        for (int i = 0; i <= conIndex; i++)
        {
            if (i == 0)
            {
                AddNewMonsterDummy();
            }
            else
            {
                dummies.Add(Instantiate(dummyPrefab, spawnPositions.poses[i].position, Quaternion.identity));
                dummies[dummies.Count - 1].AddComponent<Dummy>().index = dummies.Count - 1;
                transformData.Add(new TransformData());
            }
        }

        dummies.Add(Instantiate(playerPrefab, spawnPositions.poses[conIndex + 1].position, Quaternion.identity));
        transformData.Add(new TransformData());
    }

    private void AddNewDummy()
    {
        dummies.Add(Instantiate(dummyPrefab, spawnPositions.poses[dummies.Count + 1].position, Quaternion.identity));
        dummies[dummies.Count - 1].AddComponent<Dummy>().index = dummies.Count - 1;
        transformData.Add(new TransformData());
    }

    private void AddNewMonsterDummy()
    {
        dummies.Add(Instantiate(dummyPredatorPref, predSpawnPos.position, Quaternion.identity));
        transformData.Add(new TransformData());
    }

    private void AddMonster()
    {
        dummies.Add(Instantiate(predatorPref, predSpawnPos.position, Quaternion.identity));
        transformData.Add(new TransformData());
    }

    private void PrintLastPacket()
    {
        try
        {
            if (newPacket)
            {
                //OnScreenConsole.Instance.Print(lastPacket);
                newPacket = false;
            }
        }
        catch { }
    }

    private void GetScared()
    {
        print("getting scared");
        dummies[conIndex + 1].GetComponent<PlayerControl>().GetScared();
    }
}
