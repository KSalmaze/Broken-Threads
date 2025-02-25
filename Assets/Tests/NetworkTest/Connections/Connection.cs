using System.Threading.Tasks;
using Tests.NetworkTest.Serializers;
using UnityEngine;

namespace Tests.NetworkTest.Connections
{
    public abstract class Connection
    {
        protected MessageInterpreter messageInterpreter = MessageInterpreter.Instance;
        protected Serializer serializer = new BinarySerializer();

        public void SendTcpMessage(Message message)
        {
            Task.Run(async () => await TCP_Send_Message(message));
        }
        
        public void SendUdpMessage(Message message)
        {
            Task.Run(async () => await UDP_Send_Message(message));
        }
        
        public abstract Task TCP_Send_Message(Message message);

        public abstract Task UDP_Send_Message(Message message);

        public abstract void Quit();
    }
}