using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicMenu : MonoBehaviour
{
    public InputField ipAddressField, nameInput;

    public PositionKeeper spawnPoses;
    public Transform predSpawnPos;
    public GameObject mpSystemPrefab;

    public MultiplayerSystem mpSystem;

    public GameObject InGameUI;
    public GameObject PersonMenu;
    public GameObject GhostMenu;

    private bool isCli;

    public void InitializeGameScene()
    {
        print("initializing game scene");

        mpSystem.spawnPositions = spawnPoses;
        mpSystem.predSpawnPos = predSpawnPos;
        mpSystem.isCli = isCli;
        mpSystem.con.ipAddr = ipAddressField.text;

        InGameUI.SetActive(true);
        PersonMenu.SetActive(false);

        mpSystem.Initialize();

        gameObject.SetActive(false);
    }

    public void HostClickHandler()
    {
        isCli = false;

        mpSystem = InstantiateMP();

        mpSystem.con = mpSystem.GetComponent<TCPServer>();
        mpSystem.player_name = nameInput.text.Length > 0 ? nameInput.text : MultiplayerSystem.GenRandString(2);

        InitializeGameScene();
    }

    public void ConnectClickHandler()
    {
        isCli = true;

        mpSystem = InstantiateMP();

        mpSystem.con = mpSystem.GetComponent<TCPClient>();
        mpSystem.player_name = nameInput.text.Length > 0 ? nameInput.text : MultiplayerSystem.GenRandString(2);

        InitializeGameScene();
    }

    public MultiplayerSystem InstantiateMP()
    {
        return Instantiate(mpSystemPrefab, null).GetComponent<MultiplayerSystem>();
    }
}
