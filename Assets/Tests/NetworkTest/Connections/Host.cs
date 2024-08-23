using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            _ = Task.Run(async () => await SearchForNewClients());
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
                    _ = Task.Run(async () => await NewClient());
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

            _ = Task.Run(async () => await Receive_TCP());
            _ = Task.Run(async () => await Receive_UDP());
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
            while (true)
            {
                UdpReceiveResult receivedResult = await _udpServer.ReceiveAsync();
                MessageInterpreter.Instance.Interpret(serializer.Deserialize<Message>(receivedResult.Buffer));
                Debug.Log("Mensagem UDP recebida");
            }
        }
        
        private async Task Receive_TCP()
        {
            while (true)
            {
                if (_playerList[0].TClient != null && _playerList[0].TClient.Available > 0)
                {
                    byte[] bytesFrom = new byte[10025];
                    await _playerList[0].TcpStream.ReadAsync(bytesFrom, 0, bytesFrom.Length);
                
                    if (bytesFrom.Length != 0)
                    {
                        MessageInterpreter.Instance.Interpret(serializer.Deserialize<Message>(bytesFrom));
                        Debug.Log("Mensagem TCP recebida");
                    }
                    else
                    {
                        Debug.Log("Error #45");
                    }
                
                    _playerList[0].TcpStream.Flush();
                }
            }
        }

        public override void Quit()
        {
            _tcpserver.Stop();
            _udpServer.Close();
        }
    }
}