using System.Threading.Tasks;

namespace Tests.NetworkTest.Connections
{
    public class Host: Connection
    {
        
        
        public async override Task TCP_Send_Message(Message message)
        {
            throw new System.NotImplementedException();
        }

        public async Task TCP_Send_Message(Message message, string player)
        {
            throw new System.NotImplementedException();
        }

        public async override Task UDP_Send_Message(Message message)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task UDP_Send_Message(Message message, string player)
        {
            throw new System.NotImplementedException();
        }

        private async Task Receive_UDP()
        {
            throw new System.NotImplementedException();
        }
        
        private async Task Receive_TCP()
        {
            throw new System.NotImplementedException();
        }
    }
}