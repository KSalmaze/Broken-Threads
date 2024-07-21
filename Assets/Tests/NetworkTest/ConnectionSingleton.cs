using System.Collections;
using System.Collections.Generic;
using System.Net;
using Tests.NetworkTest.Connections;
using UnityEngine;

public class ConnectionSingleton
{
    // Singleton
    private static ConnectionSingleton instance;

    public static ConnectionSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConnectionSingleton();
            }
            return instance;
        }
    }

    private ConnectionSingleton()
    {
    }
    
    // Informacoes de Player
    public string Player_Name;
    public IPAddress Player_IP;

    // Conection
    public Connection Connection;
}
