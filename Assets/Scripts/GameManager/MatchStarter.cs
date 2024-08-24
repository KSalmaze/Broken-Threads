using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tests.NetworkTest.Connections;
using UnityEngine;

public class MatchStarter : MonoBehaviour
{
    [SerializeField] private Transform _hostSpawn;
    [SerializeField] private Transform _clientSpawn;

    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject RivalPrefab;
    
    
    // Start is called before the first frame update
    void Start()
    {
        SetPosition();
    }

    public void SetPosition()
    {
        Debug.Log("IniciandoMatchStarter");
        if (ConnectionSingleton.Instance.Connection is Client client)
        {
            Debug.Log("Client detectado");
            PlayerPrefab.transform.position = _clientSpawn.position;
            PlayerPrefab.transform.rotation = _clientSpawn.rotation;

            RivalPrefab.transform.position = _hostSpawn.position;
            RivalPrefab.transform.rotation = _hostSpawn.rotation;

        }
        else
        {
            Debug.Log("Host detectado");
            PlayerPrefab.transform.position = _hostSpawn.position;
            PlayerPrefab.transform.rotation = _hostSpawn.rotation;

            RivalPrefab.transform.position = _clientSpawn.position;
            RivalPrefab.transform.rotation = _clientSpawn.rotation;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
