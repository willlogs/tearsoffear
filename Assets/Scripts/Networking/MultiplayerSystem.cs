using System;
using System.Collections.Generic;
using UnityEngine;

/// 
/// TODO:
/// 1- Spawn Players and dummies x
/// 2- Add Menu to choose to host or connect to some other host x
/// 3- Multiple connection handling from in server x
///     3-1- Every one spawns in the same position
///     3-2- Second client's messages won't get to the first from the server
/// 4- Don't send redundant data, check for changes only x
/// 

public class MultiplayerSystem : MonoBehaviour
{
    public GameObject playerPrefab, dummyPrefab;
    public PositionKeeper spawnPositions;

    public List<GameObject> dummies = new List<GameObject>();
    public List<TransformData> transformData = new List<TransformData>();
    public TCPClient cli;
    public TCPServer srv;
    public bool isCli, isTest;
    public int conIndex = 0; // conIndex + 1 is the index of the dummy in the scene (if isCli is on) - index of srv is 0

    delegate void action();
    List<action> actions = new List<action>();

    bool connected = false;
    int connections = 0;
    TransformData td;

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

                dummies.Add(Instantiate(playerPrefab, position: spawnPositions.poses[0].position, Quaternion.identity));
                transformData.Add(new TransformData());

                StartServer();
            }
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

            if (td.isSet && !shouldSkip)
            {
                dummies[i].transform.rotation = td.rotation;
                dummies[i].transform.position = td.position;
                td.isSet = false;
            }            

            i++;
        }
    }

    private void SetTD(Packet p)
    {
        try
        {
            if ((isCli && p.index != conIndex + 1) || (!isCli && p.index != 0))
            {
                TransformData td = p.td;
                td.isSet = true;
                transformData[p.index] = td;
            }
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
                print(data);
                srv.BroadCastMessage(data);
            }
            Packet p = JsonUtility.FromJson<Packet>(data);
            if (p.type == PacketType.NEWCLI)
            {
                actions.Add(AddNewDummy);
            }
            else
            {
                if (p.tdSet)
                {
                    SetTD(p);
                }
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
        transformData.Add(new TransformData());
    }

    private void InstantiateCli()
    {
        for (int i = 0; i <= conIndex; i++)
        {
            dummies.Add(Instantiate(dummyPrefab, spawnPositions.poses[i].position, Quaternion.identity));
            transformData.Add(new TransformData());
        }

        dummies.Add(Instantiate(playerPrefab, spawnPositions.poses[conIndex + 1].position, Quaternion.identity));
    }

    private void AddNewDummy()
    {
        dummies.Add(Instantiate(dummyPrefab, spawnPositions.poses[dummies.Count + 1].position, Quaternion.identity));
        transformData.Add(new TransformData());
    }
}
