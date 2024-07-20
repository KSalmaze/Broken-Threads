using System.Collections;
using System.Collections.Generic;
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
    public string Player_IP;

    // Conection
}
