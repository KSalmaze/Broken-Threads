using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tests.NetworkTest.Connections;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField playerName2;
    
    public void SwitchActiveObject(GameObject actual)
    {
        actual.SetActive(!actual.activeSelf);
    }

    public void HostIP(TMP_InputField inputField)
    {
        Debug.Log("Tentando abrir conexão ->" + inputField.text + "<-");
        if (IsValidIPAddress(inputField.text) && PlayerName() != String.Empty)
        {
            DefinirNome(PlayerName());
            Debug.Log("Ip validado");
            ConnectionSingleton.Instance.Player_IP = IPAddress.Parse(inputField.text);
            Host host = (ConnectionSingleton.Instance.Connection = new Host()) as Host;
            Debug.Log("host criado");
            Task.Run(async () => await host.OpenServer(inputField.text));
        }
    }

    public void ClientIP(TMP_InputField inputField)
    {
        Debug.Log("Tantando abrir conexão");
        if (IsValidIPAddress(inputField.text) && PlayerName() != String.Empty)
        {
            DefinirNome(PlayerName());
            Debug.Log("Ip validado");
            ConnectionSingleton.Instance.Player_IP = IPAddress.Parse(inputField.text);
            Client client = (ConnectionSingleton.Instance.Connection = new Client()) as Client;
            Task.Run(async () => await client.Connect(inputField.text, 5020));
        }
    }

    private void DefinirNome(string nome)
    {
        ConnectionSingleton.Instance.Player_Name = nome;
    }
    
    static bool IsValidIPAddress(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }

    void OnApplicationQuit()
    {
        ConnectionSingleton.Instance.Connection.Quit();
    }

    private string PlayerName()
    {
        if (!string.IsNullOrEmpty(playerName.text))
        {
            return playerName.text;
        }
        if (!string.IsNullOrEmpty(playerName2.text))
        {
            return playerName2.text;
        }
        
        return String.Empty;
    }
}
