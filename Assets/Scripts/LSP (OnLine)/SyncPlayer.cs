using Tests.NetworkTest.Serializers;
using UnityEngine;

namespace LSP__OnLine_
{
    public class SyncPlayer : MonoBehaviour
    {
        private Transform _transform;
        private Serializer _serializer;
        private int _counter;
    
        // Start is called before the first frame update
        void Start()
        {
            _counter = 0;
            _transform = gameObject.GetComponent<Transform>();
            _serializer = new BinarySerializer();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_counter > 8)
            {
                Debug.Log("Enviando Mensagem");
                ConnectionSingleton.Instance.Connection.SendUdpMessage(
                    new Message("POS", _serializer.Serialize(new TransformData(_transform))));
            }
            else
            {
                _counter++;
            }
        }
    }
}
