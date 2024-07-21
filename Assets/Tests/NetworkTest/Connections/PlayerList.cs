using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using JetBrains.Annotations;

namespace Tests.NetworkTest.Connections
{
    public class PlayerList: List<Player>
    {
        public List<NetworkStream> AllPlayersTcpStream;
        public List<IPEndPoint> AllPlayerEndPoint;
        
        private List<NetworkStream> Update_Players_TCP_Stream()
        {
            List<NetworkStream> networkStreams = new List<NetworkStream>();

            foreach (Player player in this)
            {
                networkStreams.Add(player.TcpStream);
            }

            return networkStreams;
        }

        private List<IPEndPoint> Update_Players_UDP_Endpoints()
        {
            List<IPEndPoint> endPoints = new List<IPEndPoint>();

            foreach (Player player in this)
            {
                endPoints.Add(player.UDPEndpoint);
            }

            return endPoints;
        }

        public void Update_Player_Connections()
        {
            AllPlayerEndPoint = Update_Players_UDP_Endpoints();
            AllPlayersTcpStream = Update_Players_TCP_Stream();
        }
    }
}

public struct Player
{
    [CanBeNull] public IPAddress IPAddress;
    public string Name;
    [CanBeNull] public NetworkStream TcpStream;
    [CanBeNull] public IPEndPoint UDPEndpoint;
    [CanBeNull] public TcpClient TClient;

    public Player(string name, IPAddress ip = null, NetworkStream tcpstream = null, IPEndPoint udpendpoint = null, TcpClient tcpClient = null)
    {
        Name = name;
        IPAddress = ip;
        TcpStream = tcpstream;
        UDPEndpoint = udpendpoint;
        TClient = tcpClient;
    }
}