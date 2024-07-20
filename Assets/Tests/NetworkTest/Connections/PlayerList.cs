using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tests.NetworkTest.Connections
{
    public class PlayerList: List<Player>
    {
        public List<NetworkStream> All_Players_TCP_Stream;
        public List<IPEndPoint> All_Player_End_Point;
        
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
            All_Player_End_Point = Update_Players_UDP_Endpoints();
            All_Players_TCP_Stream = Update_Players_TCP_Stream();
        }
    }
}

public struct Player
{
    public IPAddress IPAddress;
    public string Name;
    public NetworkStream TcpStream;
    public IPEndPoint UDPEndpoint;
}