using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tests.NetworkTest.Connections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    
    public void SwitchActiveObject(GameObject gameo)
    {
        gameo.SetActive(!gameo.activeSelf);
    }

    public void SetActive(GameObject gameo)
    {
        gameo.SetActive(true);
    }
    
    public void SetFalse(GameObject gameo)
    {
        gameo.SetActive(false);
    }

    public void HostButton(TMP_InputField inputField)
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

    public void ClientButton(TMP_InputField inputField)
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
        
        return String.Empty;
    }

    public void StartButton()
    {
        ConnectionSingleton.Instance.Connection.TCP_Send_Message(new Message("START", new Byte[]{0}));
        SceneManager.LoadScene("Map");
    }
}
