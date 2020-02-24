using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData
{
    public Quaternion rotation;
    public Vector3 position;
    public Vector3 rbVelocity;

    public TransformData(Quaternion r, Vector3 p, Vector3 vel)
    {
        rotation = r;
        position = p;
        rbVelocity = vel;
    }
}

public class MultiplayerSystem : MonoBehaviour
{
    public GameObject effected, reference;
    public TCPClient cli;
    public TCPServer srv;
    public bool isCli, isTest;

    Rigidbody effectedRB, referenceRB;
    TransformData transformData;
    bool tdSet = false, connected = false;
    int connections = 0;

    private void Start()
    {
        effectedRB = effected.GetComponent<Rigidbody>();
        referenceRB = reference.GetComponent<Rigidbody>();

        if (isTest)
        {
            cli.OnRecieveData += OnRecieveData;
            srv.OnRecieveData += OnRecieveData;

            cli.OnConnected += OnConnectionStablished;
            srv.OnConnected += OnConnectionStablished;

            StartServer();

            cli.StartConnection();
        }
        else
        {
            if (isCli)
            {
                cli.OnRecieveData += OnRecieveData;
                cli.OnConnected += OnConnectionStablished;
                cli.StartConnection();
            }
            else
            {
                srv.OnRecieveData += OnRecieveData;
                srv.OnConnected += OnConnectionStablished;

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
        if (!isTest && connected)
        {
            TransformData td = new TransformData(reference.transform.rotation, reference.transform.position, referenceRB.velocity);

            if (isCli)
            {
                cli.SendMessage_(JsonUtility.ToJson(td));
            }
            else
            {
                srv.SendMessage_(JsonUtility.ToJson(td));
            }

            if (tdSet)
            {
                effected.transform.rotation = transformData.rotation;
                effected.transform.position = transformData.position;
                effectedRB.velocity = transformData.rbVelocity;
                tdSet = false;
            }
        }
    }

    private void OnRecieveData(string data)
    {
        OnScreenConsole.Instance.Print("Recieved " + data);
        if (isTest)
        {
            print(data);
        }
        else
        {
            TransformData td = JsonUtility.FromJson<TransformData>(data);
            transformData = td;
            tdSet = true;
        }
    }

    private void OnConnectionStablished(string data)
    {
        if (isTest)
        {
            connections++;
            print("new connection stablished : " + connections);
            if (connections == 2)
            {
                srv.SendMessage_("Hi!");
                cli.SendMessage_("Hi from cli!");
            }
        }
        else
        {
            connected = true;
        }
    }
}
