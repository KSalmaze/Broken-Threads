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
    
    public void SwitchActiveObject(GameObject actual)
    {
        actual.SetActive(!actual.activeSelf);
    }

    public void HostIP(TMP_InputField inputField)
    {
        DefinirNome();
        Debug.Log("Tentando abrir conexão ->" + inputField.text + "<-");
        if (IsValidIPAddress(inputField.text))
        {
            Debug.Log("Ip validado");
            Host host = (ConnectionSingleton.Instance.Connection = new Host()) as Host;
            Debug.Log("host criado");
            Task.Run(async () => await host.OpenServer(inputField.text));
        }
    }

    public void ClientIP(TMP_InputField inputField)
    {
        DefinirNome();
        Debug.Log("Tantando abrir conexão");
        if (IsValidIPAddress(inputField.text))
        {
            Debug.Log("Ip validado");
            Client client = (ConnectionSingleton.Instance.Connection = new Client()) as Client;
            Task.Run(async () => await client.Connect(inputField.text, 5020));
        }
    }

    private void DefinirNome()
    {
        ConnectionSingleton.Instance.Player_Name = playerName.text;
    }
    
    static bool IsValidIPAddress(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }

    void OnApplicationQuit()
    {
        ConnectionSingleton.Instance.Connection.Quit();
    }
}
