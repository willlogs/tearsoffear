using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicMenu : MonoBehaviour
{
    public InputField ipAddressField;

    public PositionKeeper spawnPoses;
    public GameObject mpSystemPrefab;

    public void HostClickHandler()
    {
        MultiplayerSystem mpSystem = InstantiateMP();

        mpSystem.spawnPositions = spawnPoses;
        mpSystem.isCli = false;

        mpSystem.Initialize();

        gameObject.SetActive(false);
    }

    public void ConnectClickHandler()
    {
        MultiplayerSystem mpSystem = InstantiateMP();

        mpSystem.spawnPositions = spawnPoses;
        mpSystem.isCli = true;
        mpSystem.cli.ipAddr = ipAddressField.text;

        mpSystem.Initialize();

        gameObject.SetActive(false);
    }

    public MultiplayerSystem InstantiateMP()
    {
        return Instantiate(mpSystemPrefab, null).GetComponent<MultiplayerSystem>();
    }
}
