using System.Threading.Tasks;
using UnityEngine;

namespace Tests.NetworkTest.Connections
{
    public abstract class Connection
    {
        public abstract Task TCP_Send_Message(Message message);

        public abstract Task UDP_Send_Message(Message message);
    }
}