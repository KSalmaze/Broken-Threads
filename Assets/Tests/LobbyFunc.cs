using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.NetworkTest.Connections;
using UnityEngine;
using TMPro;

public class LobbyFunc : MonoBehaviour
{
    [SerializeField] private TMP_Text fClient, fHost;
    [SerializeField] private List<GameObject> lobbyScene;
    [SerializeField] private GameObject startMatchButton;
    
    void Start()
    {
        MessageInterpreter.Instance.AddFunction("NEW",InitalClientConnection);
        MessageInterpreter.Instance.AddFunction("LOBBY", GoToLobby);
        MessageInterpreter.Instance.AddFunction("IGN", Ignore);
        MessageInterpreter.Instance.AddFunction("START", StartMatch);
    }

    private void GoToLobby(byte[] bytes, string user)
    {
        fClient.text = user;
        fHost.text = user;

        if (ConnectionSingleton.Instance.Connection is Client client)
        {
            _ = Task.Run(async () => await client.TCP_Send_Message(new Message("LOBBY", new Byte[]{})));
            foreach (GameObject go in lobbyScene)
            {
                go.SetActive(true);
            }
        }
        else
        {
            // Permite iniciar a partida
        }
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

    private void StartMatch(byte[] bytes, string user)
    {
        
    }
}
