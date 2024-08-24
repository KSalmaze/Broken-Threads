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
            client.Connect(_hostip, port);
            while (!client.Connected){}
            stream = client.GetStream();
            Task.Run(async () => await Receive_TCP());
            
            Debug.Log("TCP Conectado");
        }

        public void Connect_to_UDP(byte[] bytes)
        {
            try{
                Debug.Log("Iniciando conexão UDP");

                udp_client = new UdpClient();
                udp_client.Connect(serializer.Deserialize<IPAddress>(bytes), _port + 1);
                Task.Run(async () => await UDP_Send_Message(new Message("IGN", new byte[] { })));

                Task.Run(async () => await Receive_UDP());
            }
            catch(Exception ex)
            {
                Debug.LogError($"Erro durante o processo de conexão: {ex.Message}");
            }
            
            TestConnection();
        }
        
        public override async Task TCP_Send_Message(Message message)
        {
            byte[] bytesToSend = serializer.Serialize(message);
            stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        public override async Task UDP_Send_Message(Message message)
        {
            try{
                Debug.Log("Enviando mensagem UDP");
                byte[] messageBytes = serializer.Serialize(message);
                // Debug.Log(udp_client.Available);
                // Debug.Log(udp_client);
                udp_client.Send(messageBytes, messageBytes.Length);
                Debug.Log("Mensagem UDP enviada");
            }
            catch(Exception ex)
            {
                // Debug.LogError($"Erro durante o envio de mensagem UDP: {ex.Message}");
            }
        }
        
        private async Task Receive_UDP()
        {
            while (true)
            {
                UdpReceiveResult udpReceiveResult = await udp_client.ReceiveAsync();
                Debug.Log("Menssagem UDP recebida");
                messageInterpreter.Interpret(serializer.Deserialize<Message>(udpReceiveResult.Buffer));
            }
        }
        
        private async Task Receive_TCP()
        {
            while (true)
            {
                try{
                    if (client.Connected && stream.CanRead)
                    {
                       // Debug.Log("Nenhuma mensagem");

                        byte[] bytesFrom = new byte[10500];
                        stream.Read(bytesFrom, 0, bytesFrom.Length);

                       // Debug.Log("MSG from " + serializer.Deserialize<Message>(bytesFrom).User);

                        if (bytesFrom.Length != 0)
                        {
                           // Debug.Log("Mensagem TCP recebida");
                            messageInterpreter.Interpret(serializer.Deserialize<Message>(bytesFrom));
                        }

                        stream.Flush();
                    }
                }catch(Exception ex)
                {
                    // Debug.LogError($"Erro durante o recebimento de uma mensagem TCP: {ex.Message}");
                }
            }
        }

        private void TestConnection()
        {
            Task.Run(async () => await UDP_Send_Message(new Message("IGN",new byte[]{0})));
            Task.Run(async () => await TCP_Send_Message(new Message("IGN",new byte[]{0})));
        }
        
        public override void Quit()
        {
            udp_client.Close();
            client.Close();
        }
    }
}