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
        MessageInterpreter.Instance.AddFunction("NEW",InitalClientConnection);
        MessageInterpreter.Instance.AddFunction("LOBBY", GoToLobby);
        MessageInterpreter.Instance.AddFunction("IGN", Ignore);
    }

    private void GoToLobby(byte[] bytes, string user)
    {
        otherPlayer.text = user;
    }

    private void InitalClientConnection(byte[] bytes,string user)
    {
        if (ConnectionSingleton.Instance.Connection is Client client)
        {
            client.Connect_to_UDP(bytes);
        }
    }

    private void Ignore(byte[] bytes, string user)
    {
        Debug.Log("TAG Ignorada");
    }
}
