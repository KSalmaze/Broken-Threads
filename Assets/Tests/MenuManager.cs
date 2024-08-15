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
    public void SwitchActiveObject(GameObject actual)
    {
        actual.SetActive(!actual.activeSelf);
    }

    public void HostIP(TMP_InputField inputField)
    {
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
        Debug.Log("Tantando abrir conexão");
        if (IsValidIPAddress(inputField.text))
        {
            Debug.Log("Ip validado");
            Client client = (ConnectionSingleton.Instance.Connection = new Client()) as Client;
            client.Connect(inputField.text, 5020);
        }
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
