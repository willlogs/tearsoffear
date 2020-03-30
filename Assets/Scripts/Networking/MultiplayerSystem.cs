using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    #region Public Variables
    public static MultiplayerSystem instance;
    public static char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

    public static string RandString(int length_)
    {
        string o = "";

        for(int i = 0; i < length_; i++)
        {
            o += chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        return o;
    }

    // Prefabs and SpawnPoses
    public delegate void action(int input);
    public delegate void strAction(string str);

    public GameObject playerPrefab, dummyPrefab, predatorPref, dummyPredatorPref;
    public PositionKeeper spawnPositions;
    public Transform predSpawnPos;

    public List<TCPMessage> messageBuffer = new List<TCPMessage>();

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

    // Message Timeout
    float timeout = 2;
    int windowSize = 5;
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
        InvokeRepeating(nameof(FlushMessages), 0, 0.2f);
        instance = this;
    }

    #region Send Packets
    public void SendToggleUVPacket()
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), type: PacketType.UV);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));
    }

    public void SendScarePacket(int targetIndex)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), sIndex: targetIndex, type: PacketType.SCARE);

        messageBuffer.Add( new TCPMessage(JsonUtility.ToJson(p), p.hash) );
    }

    public void SendDoorTogglePacket(string name)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), targetString: name, type: PacketType.DOORTOGGLE);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));
    }

    public void SendCollectPacket(string name)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), targetString: name, type: PacketType.COLLECT);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));
    }

    public void SendVisPacket(bool isvis)
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), type: isvis?PacketType.VIS:PacketType.INVIS);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));

        OnScreenConsole.Instance.Print("vis packet sent " + isvis);
    }

    public void SendFlashLightPacket()
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), type: PacketType.FLASHTOGGLE);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));
    }

    public void SendHitPacket()
    {
        int index = !isCli ? 0 : conIndex + 1;

        Packet p = new Packet(index, RandString(10), type: PacketType.HIT, sIndex: 0);

        messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), p.hash));
    }
    #endregion

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
    }

    private void DoActions()
    {
        if (actions.Count > 0)
        {
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i].isStr)
                {
                    actions[i].sa(actions[i].sinput);
                }
                else
                {
                    actions[i].a(actions[i].input);
                }
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
            Packet p = new Packet(index, "", td: td, tdSet: true);

            // if you want to make it totally multiplayer change this part for the server : broadcast
            messageBuffer.Add(new TCPMessage(JsonUtility.ToJson(p), ""));
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
        foreach(string s in Packer.Parse(data))
        {
            ParseMessage(s);
        }
    }

    private void ParseMessage(string data)
    {
        try
        {
            Packet p = JsonUtility.FromJson<Packet>(data);

            if (p.type != PacketType.TRANSFORMDATA)
            {
                lastPacket = p.hash + " " + p.type.ToString();
                newPacket = true;
            }
            //if (!isCli)
            //    con.BroadCastMessage(data, p.index - 1);

            switch (p.type)
            {
                case PacketType.TRANSFORMDATA:
                    if (p.tdSet)
                    {
                        SetTD(p);
                        actions.Add(new Action(0, UpdateTD));
                    }
                    break;

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

                case PacketType.SCARE:
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
                    actions.Add(new Action(p.targetString, ToggleDoor));
                    break;

                case PacketType.COLLECT:
                    actions.Add(new Action(p.targetString, CollectCollectable));
                    break;

                case PacketType.VIS:
                    if (!isCli)
                    {
                        print("vis");
                        actions.Add(new Action(0, SetVisibility));
                    }
                    break;

                case PacketType.INVIS:
                    if (!isCli)
                    {
                        print("vis");
                        actions.Add(new Action(1, SetVisibility));
                    }
                    break;

                case PacketType.FLASHTOGGLE:
                    int index = !isCli ? 0 : conIndex + 1;
                    if (index != p.index)
                    {
                        actions.Add(new Action(p.index, SwitchFlashlight));
                    }
                    break;

                case PacketType.UV:
                    actions.Add(new Action(p.index, UvSet));
                    break;
            }
        }
        catch (Exception e)
        {
            print("parsing err: " + e);
            print(data);
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
            Packet p = new Packet(conIndex, "", type: PacketType.NEWCLI);
            con.BroadCastMessage(JsonUtility.ToJson(p), conIndex);
        }
        connected = true;
    }

    private void FlushMessages()
    {
        if (connected)
        {
            SendTD();
            if (messageBuffer.Count > 0)
            {
                SendMessageTo(Packer.Pack(messageBuffer.ToArray()));
                messageBuffer.Clear();
            }
        }
    }

    private void SendMessageTo(string mssg, int i = 0)
    {
        con.SendMessage_(mssg, i);
    }

    private void SendMessageTo(TCPMessage mssg, int i = 0)
    {
        print("sending");
        mssg.timerOn = true;
        con.SendMessage_(mssg.mssg, i);
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
        PlayerControl pc = (PlayerControl)tools.myController;

        if (pc.shielded)
        {
            SendHitPacket();
        }
        else
        {
            pc.GetScared();
        }
    }

    private void ToggleDoor(string name)
    {
        DoorManager.Toggle(name);
    }

    private void CollectCollectable(string name)
    {
        Collectible.GetCollected(name);
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

    private void SwitchFlashlight(int input)
    {
        tools.dummies[input].GetComponent<DummyPlayer>().ToggleFlashLight();
    }

    private void UvSet(int input)
    {
        tools.dummies[input].GetComponent<DummyPlayer>().UVToggle();
    }

    private void UpdateTD(int input)
    {
        tools.UpdateTransforms(isCli, conIndex);
    }
}
