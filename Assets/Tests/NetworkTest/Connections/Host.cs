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
            try {
                Debug.Log("Iniciando conexão com client");

                TcpClient client = _tcpserver.AcceptTcpClient();
                Debug.Log("Client get");
                NetworkStream clientStream = client.GetStream();
                Debug.Log("TCP Connection accepted");
                
                byte[] bytesToSend = serializer.Serialize(new Message("NEW", serializer.Serialize(ConnectionSingleton.Instance.Player_IP)));
                // await Task.Delay(2000);
                Debug.Log(clientStream.CanWrite);
                while (!clientStream.CanWrite) {}
                clientStream.Write(bytesToSend, 0, bytesToSend.Length);
                Debug.Log("TCP Message sended");

                UdpReceiveResult receivedResult = await _udpServer.ReceiveAsync();
                IPEndPoint remoteEndPoint = receivedResult.RemoteEndPoint;
                Debug.Log("UDP Connection accepted");
                string playerName = serializer.Deserialize<Message>(receivedResult.Buffer).User;
                _playerList.Add(new Player(playerName, tcpClient: client, tcpstream: clientStream,
                    udpendpoint: remoteEndPoint));
                _serverLivre = true;

                _playerList.Update_Player_Connections();
                Debug.Log("Conexão estabelecida com sucesso");

                _ = Task.Run(async () => await Receive_TCP());
                _ = Task.Run(async () => await Receive_UDP());

                TestConnection();
            }
            catch(Exception ex)
            {
                Debug.LogError($"Erro durante o processo de conexão: {ex.Message}");
            }
        }
        
        public override async Task TCP_Send_Message(Message message)
        {
            byte[] bytesToSend = serializer.Serialize(message);

            foreach (NetworkStream clientStream in _playerList.AllPlayersTcpStream)
            {
                clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
            }
        }

        public async Task TCP_Send_Message(Message message, string player)
        {
            throw new System.NotImplementedException();
        }

        public override async Task UDP_Send_Message(Message message)
        {
            try
            {
                byte[] bytesToSend = serializer.Serialize(message);
            
                foreach (IPEndPoint ipEndPoint in _playerList.AllPlayerEndPoint)
                {
                    await _udpServer.SendAsync(bytesToSend, bytesToSend.Length, ipEndPoint);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Erro durante o envio de mensagem UDP: {ex.Message}");
            }
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
                Debug.Log("Mensagem UDP recebida");
                MessageInterpreter.Instance.Interpret(serializer.Deserialize<Message>(receivedResult.Buffer));
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

        private void TestConnection()
        {
            Task.Run(async () => await UDP_Send_Message(new Message("IGN",new byte[]{0})));
            Task.Run(async () => await TCP_Send_Message(new Message("IGN",new byte[]{0})));
            Task.Run(async () => await TCP_Send_Message(new Message("LOBBY",new byte[]{0})));
        }
        
        public override void Quit()
        {
            _tcpserver.Stop();
            _udpServer.Close();
        }
    }
}