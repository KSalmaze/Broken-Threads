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
    private MessageInterpreter.Func funcao;
    private byte[] _bytes;
    private string _user;
    private bool run = false;
    
    private void Update()
    {
        if (run)
        {
            funcao(_bytes, _user);
            run = false;
        }
    }

    void Start()
    {
        MessageInterpreter.Instance.AddFunction("NEW",InitalClientConnection);
        MessageInterpreter.Instance.AddFunction("LOBBY", Lobby);
        MessageInterpreter.Instance.AddFunction("IGN", Ignore);
        MessageInterpreter.Instance.AddFunction("START", StartMatch);
    }

    private void StartMatch(byte[] bytes, string user)
    {
        
    }
    
    private void Lobby(byte[] bytes, string user)
    {
        funcao = GoToLobby;
        _bytes = bytes;
        _user = user;
        run = true;
    }
    
    private void GoToLobby(byte[] bytes, string user)
    {
        Debug.Log("GotoLobby");
        fClient.text = user;
        Debug.Log("Nome do client escrito");
        fHost.text = user;
        Debug.Log("Nome do host escrito");

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
            startMatchButton.SetActive(true);
        }
        
        Debug.Log("Finaizado");
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
