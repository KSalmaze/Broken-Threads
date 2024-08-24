using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Tests.NetworkTest.Serializers;
using UnityEngine;

public class MatchFunc : MonoBehaviour
{
    [SerializeField] private Transform oponente;

    private MatchStarter _matchStarter;
    private MessageInterpreter.Func funcao;
    private byte[] _bytes;
    private string _user;
    private bool run = false;
    
    private Serializer _serializer;
    
    // Start is called before the first frame update
    void Start()
    {
        _matchStarter = gameObject.GetComponent<MatchStarter>();
        _serializer = new BinarySerializer();
        MessageInterpreter.Instance.AddFunction("POS", OP);
        MessageInterpreter.Instance.AddFunction("HIT", H);
        MessageInterpreter.Instance.AddFunction("DIE",D);
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

    void D(byte[] bytes, string user)
    {
        funcao = Die;
        _bytes = bytes;
        _user = user;
        run = true;
    }
    
    void OponentePosition(byte[] bytes, string user)
    {
        Debug.Log("Menssagem com tag POS sendo interpretada");
        TransformData transData = _serializer.Deserialize<TransformData>(bytes);
        oponente.position = new Vector3(transData.position[0],transData.position[1],transData.position[2]);
    }

    void Hit(byte[] bytes, string user)
    {
        
    }

    void Die(byte[] bytes, string user)
    {
        _matchStarter.SetPosition();
    }
}
