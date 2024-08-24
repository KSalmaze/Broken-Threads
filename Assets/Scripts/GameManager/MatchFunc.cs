using System.Collections;
using System.Collections.Generic;
using Tests.NetworkTest.Serializers;
using UnityEngine;

public class MatchFunc : MonoBehaviour
{
    [SerializeField] private Transform oponente;
    
    private MessageInterpreter.Func funcao;
    private byte[] _bytes;
    private string _user;
    private bool run = false;
    
    private Serializer _serializer;
    
    // Start is called before the first frame update
    void Start()
    {
        _serializer = new BinarySerializer();
        MessageInterpreter.Instance.AddFunction("POS", OP);
        MessageInterpreter.Instance.AddFunction("HIT", H);
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            funcao(_bytes, _user);
            run = false;
        }
    }

    void OP(byte[] bytes, string user)
    {
        funcao = OponentePosition;
        _bytes = bytes;
        _user = user;
        run = true;
    }
    void H(byte[] bytes, string user)
    {
        funcao = Hit;
        _bytes = bytes;
        _user = user;
        run = true;
    }
    
    void OponentePosition(byte[] bytes, string user)
    {
        oponente = _serializer.Deserialize<Transform>(bytes);
    }

    void Hit(byte[] bytes, string user)
    {
        
    }
}
