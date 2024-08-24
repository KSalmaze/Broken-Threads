using System.Threading.Tasks;
using Tests.NetworkTest.Serializers;
using UnityEngine;

namespace Tests.NetworkTest.Connections
{
    public abstract class Connection
    {
        protected MessageInterpreter messageInterpreter = MessageInterpreter.Instance;
        protected Serializer serializer = new BinarySerializer();
        
        public abstract Task TCP_Send_Message(Message message);

        public abstract Task UDP_Send_Message(Message message);

        public abstract void Quit();
    }
}