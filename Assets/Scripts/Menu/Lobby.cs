using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public GameObject LobbyMenu;

    public Text nameShower;

    public void StartButtonHandler()
    {
        if (!MultiplayerSystem.instance.isCli)
        {
            MultiplayerSystem.instance.SendStartGamePacket();
            StartGame();
        }
    }

    public void StartGame()
    {
        LobbyMenu.SetActive(false);
        FindObjectOfType<Controller>().GetAlive();
        MultiplayerSystem.instance.StopListening();
    }

    public void NewMember(string name)
    {
        nameShower.text = nameShower.text + "\n" + name;
    }
}
