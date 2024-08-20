using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.NetworkTest.Connections
{
    public class Host: Connection
    {
        private PlayerList _playerList;

        public Host()
        {
            _playerList = new PlayerList();
        }
        
        // Server
        private TcpListener _tcpserver;
        private UdpClient _udpServer;
        
        public async Task OpenServer(string ip, int port = 5020)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            ConnectionSingleton.Instance.Player_IP = ipAddress;
            _tcpserver = new TcpListener(ipAddress, port);
            _tcpserver.Start();
            Debug.Log("Server tcp aberto");
            _udpServer = new UdpClient(new IPEndPoint(ipAddress, port + 1));
            Debug.Log("Server udp aberto");
            _acceptNewClients = true;
            _serverLivre = true;
            Task.Run(async () => await SearchForNewClients());
            Debug.Log("Server funcional");
        }

        private bool _acceptNewClients;
        private bool _serverLivre = true;
            
        private async Task SearchForNewClients()
        {
            while (_acceptNewClients)
            {
                Debug.Log(_tcpserver.Pending());
                if (_serverLivre && _tcpserver.Pending())
                {
                    Debug.Log("Aceitando client");
                    _serverLivre = false;
                    Task.Run(async () => await NewClient());
                }
            }
        }

        private async Task NewClient()
        {
            Debug.Log("Iniciando conexão com client");
            
            TcpClient client = await _tcpserver.AcceptTcpClientAsync();
            NetworkStream clientStream = client.GetStream();
            
            byte[] bytesToSend = serializer.Serialize(new Message("IGN", new byte[]{0}));
            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
            
            UdpReceiveResult receivedResult = await _udpServer.ReceiveAsync();
            IPEndPoint remoteEndPoint = receivedResult.RemoteEndPoint;
            Console.WriteLine("Connection accepted");
            string playerName = serializer.Deserialize<Message>(receivedResult.Buffer).User;
            _playerList.Add(new Player(playerName, tcpClient: client, tcpstream: clientStream, udpendpoint: remoteEndPoint));
            _serverLivre = true;
            
            Debug.Log("Conexão estabelecida com sucesso");
        }
        
        public async override Task TCP_Send_Message(Message message)
        {
            byte[] bytesToSend = serializer.Serialize(message);

            foreach (NetworkStream clientStream in _playerList.AllPlayersTcpStream)
            {
                clientStream.Write(bytesToSend, 0, bytesToSend.Length);
            }
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
            // A primeira vez será para inicializar o End point apenas depois a veria
            // um receive normal
            throw new System.NotImplementedException();
        }
        
        private async Task Receive_TCP()
        {
            throw new System.NotImplementedException();
        }

        public override void Quit()
        {
            _tcpserver.Stop();
            _udpServer.Close();
        }
    }
}