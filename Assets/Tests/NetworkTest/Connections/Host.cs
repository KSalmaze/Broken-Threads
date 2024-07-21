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
        
        public async Task OpenServer(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            ConnectionSingleton.Instance.Player_IP = ipAddress;
            _tcpserver = new TcpListener(ipAddress, port);
            _udpServer = new UdpClient(new IPEndPoint(ipAddress, port + 1));
            _acceptNewClients = true;
            Task.Run(async () => await SearchForNewClients());
        }

        private bool _acceptNewClients;
        private bool _serverLivre = true;
            
        private async Task SearchForNewClients()
        {
            while (_acceptNewClients)
            {
                if (_serverLivre && _tcpserver.Pending())
                {
                    _serverLivre = false;
                    Task.Run(async () => await NewClient());
                }
            }
        }

        private async Task NewClient()
        {
            TcpClient client = await _tcpserver.AcceptTcpClientAsync();
            NetworkStream clientStream = client.GetStream();
            
            Task.Run(async () => await TCP_Send_Message(new Message("IGN", serializer.Serialize("Connect to UDP"))));
            
            byte[] bytesToSend = serializer.Serialize(new Message("IGN", new byte[]{0}));
            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
            
            UdpReceiveResult receivedResult = await _udpServer.ReceiveAsync();
            IPEndPoint remoteEndPoint = receivedResult.RemoteEndPoint;
            Console.WriteLine("Connection accepted");
            string playerName = serializer.Deserialize<Message>(receivedResult.Buffer).User;
            _playerList.Add(new Player(playerName, tcpClient: client, tcpstream: clientStream, udpendpoint: remoteEndPoint));
            _serverLivre = true;
        }
        
        public async override Task TCP_Send_Message(Message message)
        {
            throw new NotImplementedException();
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
    }
}