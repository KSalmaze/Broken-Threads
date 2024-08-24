using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using Tests.NetworkTest.Serializers;
using UnityEngine;

public class SyncPlayer : MonoBehaviour
{
    private Transform _transform;
    private Serializer _serializer;
    private int counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        _serializer = new BinarySerializer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (counter > 8)
        {
            Debug.Log("Enviando Mensagem");
            ConnectionSingleton.Instance.Connection.UDP_Send_Message(
                new Message("POS", _serializer.Serialize(new TransformData(_transform))));
        }
        else
        {
            counter++;
        }
    }
}
