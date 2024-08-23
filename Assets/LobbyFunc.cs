using System.Collections;
using System.Collections.Generic;
using Tests.NetworkTest.Connections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class LobbyFunc : MonoBehaviour
{
    [SerializeField] private TMP_Text otherPlayer;
    
    void Start()
    {
        MessageInterpreter.Instance.AddFunction("IGN",InitalClientConnection);
        MessageInterpreter.Instance.AddFunction("LOBBY", GoToLobby);
    }

    private void GoToLobby(byte[] bytes, string user)
    {
        otherPlayer.text = user;
    }

    private void InitalClientConnection(byte[] bytes,string user)
    {
        if (ConnectionSingleton.Instance.Connection is Client client)
        {
            client.UDP_Send_Message(new Message("A",new byte[]{0}));
        }
    }
}
