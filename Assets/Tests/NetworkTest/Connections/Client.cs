using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.NetworkTest.Connections
{
    public class Client: Connection
    {
        private TcpClient client;
        private UdpClient udp_client;
        private NetworkStream stream;
        
        // Host info
        private IPAddress _hostip;
        private int _port;

        public async Task Connect(string hostip, int port)
        {
            Debug.Log("Tentando conectar");
            _port = port;
            _hostip = IPAddress.Parse(hostip);
            
            client = new TcpClient();
            Debug.Log("Tentando se conectar ao server");
            await client.ConnectAsync(_hostip, port);
            stream = client.GetStream();

            Debug.Log("Conectado");
            Task.Run(async () => await Receive_TCP());
        }

        public void Connect_to_UDP()
        {
            Task.Run(async () => await Receive_UDP());
        }
        
        public override async Task TCP_Send_Message(Message message)
        {
            byte[] bytesToSend = serializer.Serialize(message);
            stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        public override async Task UDP_Send_Message(Message message)
        {
            byte[] messageBytes = serializer.Serialize(message);
            udp_client.SendAsync(messageBytes, messageBytes.Length);
        }
        
        private async Task Receive_UDP()
        {
            while (true)
            {
                UdpReceiveResult udpReceiveResult = await udp_client.ReceiveAsync();
                messageInterpreter.Interpret(serializer.Deserialize<Message>(udpReceiveResult.Buffer)); 
            }
        }
        
        private async Task Receive_TCP()
        {
            while (true)
            {
                if (client.Connected)
                {
                    Debug.Log("Recebendo mensagem");
                    byte[] bytesFrom = new byte[10500];
                    stream.Read(bytesFrom, 0, bytesFrom.Length);

                    if (bytesFrom.Length != 0)
                    {
                        messageInterpreter.Interpret(serializer.Deserialize<Message>(bytesFrom));   
                    }
                    stream.Flush();
                }
            }
        }

        public override void Quit()
        {
            throw new NotImplementedException();
        }
    }
}