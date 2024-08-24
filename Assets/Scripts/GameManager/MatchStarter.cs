using System.Collections;
using System.Collections.Generic;
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
        if (ConnectionSingleton.Instance.Connection is Client)
        {
            Transform PlayerTransform = PlayerPrefab.GetComponent<Transform>();
            PlayerTransform = _clientSpawn;
            Transform RivalTransform = RivalPrefab.GetComponent<Transform>();
            RivalTransform = _hostSpawn;
        }
        else
        {
            Transform PlayerTransform = PlayerPrefab.GetComponent<Transform>();
            PlayerTransform = _hostSpawn;
            Transform RivalTransform = RivalPrefab.GetComponent<Transform>();
            RivalTransform = _clientSpawn;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
