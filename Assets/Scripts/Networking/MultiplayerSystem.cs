using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    #region Public Variables
    public static MultiplayerSystem instance;

    // Prefabs and SpawnPoses
    public delegate void action(int input);

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
    List<Action> actions = new List<Action>();

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

    public void SendDoorTogglePacket(int targetI)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, sIndex: targetI, type: PacketType.DOORTOGGLE);

        SendMessageTo(JsonUtility.ToJson(p));
    }

    public void SendCollectPacket(int targetIndex)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, sIndex: targetIndex, type: PacketType.COLLECT);

        SendMessageTo(JsonUtility.ToJson(p));
    }

    public void SendVisPacket(bool isvis)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, type: isvis?PacketType.VIS:PacketType.UNVIS);

        SendMessageTo(JsonUtility.ToJson(p));
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
            tools.AddMonster(0);
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
                actions[i].a(actions[i].input);
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
            lastPacket = data;
            newPacket = true;

            Packet p = JsonUtility.FromJson<Packet>(data);

            if(!isCli)
                con.BroadCastMessage(data, p.index - 1);

            switch (p.type)
            {
                case PacketType.NEWCLI:
                    if (p.isPred)
                    {
                        actions.Add(new Action(-1, tools.AddDummyMonster));
                    }
                    else
                    {
                        actions.Add(new Action(-1, tools.AddDummy));
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
                        actions.Add(new Action(-1, GetScared));
                    }
                    break;

                // TODO: fix when pred is chosen randomly
                case PacketType.HIT:
                    if (!isCli)
                    {
                        tools.dummies[0].GetComponent<PredatorControl>().Die();
                    }
                    break;

                case PacketType.DOORTOGGLE:
                    actions.Add(new Action(p.targetIndex, ToggleDoor));
                    print("door toggle: " + p.targetIndex);
                    break;

                case PacketType.COLLECT:
                    actions.Add(new Action(p.targetIndex, CollectCollectable));
                    print("collecting " + p.targetIndex);
                    break;

                case PacketType.VIS:
                    if (!isCli)
                    {
                        print("vis");
                        actions.Add(new Action(0, SetVisibility));
                    }
                    break;

                case PacketType.UNVIS:
                    if (!isCli)
                    {
                        print("unvis");
                        actions.Add(new Action(1, SetVisibility));
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

        conIndex = Convert.ToInt32(data);
        actions.Add(new Action(-1, InstantiateCli));
        connected = true;
    }

    private void OnConnectionStablishedSrv(string data)
    {
        lastPacket = data;
        newPacket = true;

        conIndex = Convert.ToInt32(data);
        if (conIndex == 0)
        {
            actions.Add(new Action(-1, InstantiateSrv));
        }
        else
        {
            actions.Add(new Action(-1, tools.AddDummy));
            Packet p = new Packet(conIndex, type: PacketType.NEWCLI);
            con.BroadCastMessage(JsonUtility.ToJson(p), conIndex);
        }
        connected = true;
    }

    private void SendMessageTo(string mssg, int i = 0)
    {
        con.SendMessage_(mssg, i);
    }

    private void InstantiateSrv(int input)
    {
        tools.AddDummy(0);
    }

    private void InstantiateCli(int input)
    {
        for (int i = 0; i <= conIndex; i++)
        {
            if (i == 0)
            {
                tools.AddDummyMonster(0);
            }
            else
            {
                GameObject temp = Instantiate(dummyPrefab, spawnPositions.poses[i].position, Quaternion.identity);
                tools.dummies.Add(temp);
                temp.AddComponent<Dummy>().index = tools.dummies.Count - 1;
                tools.transformData.Add(new TransformData());
            }
        }

        tools.AddPlayer(0);
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

    private void GetScared(int input)
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

    private void ToggleDoor(int input)
    {
        DoorManager.instance.Toggle(input);
    }

    private void CollectCollectable(int input)
    {
        Collectible.GetCollected(input);
    }

    private void SetVisibility(int input)
    {
        PredatorControl pc = (PredatorControl)tools.myController;
        if(input == 0)
        {
            pc.canMove = false;
        }
        else
        {
            pc.canMove = true;
        }
    }
}
