using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Insert_Creative_Name
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        private const int BufferSize = 1024;
        private IPAddress LocalIp;
        private int LocalPort; //25252
        private IPEndPoint LocalEndPoint;
        internal Socket ServerSocket;
        internal ConcurrentDictionary<Socket, Client> Clients;
        private ConcurrentDictionary<uint, Objects.WorldObject> Objects;
        private ConcurrentDictionary<ushort, Objects.Map> Maps;
        private ConcurrentDictionary<uint, Objects.WorldMap> WorldMaps;
        internal ClientPacketHandler[] ClientPacketHandlers { get; }
        internal ServerPackets Packets { get; }

        internal Server(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new ConcurrentDictionary<Socket, Client>();
            ClientPacketHandlers = new ClientPackets().Handlers;
            Packets = new ServerPackets();
        }

        internal void Start()
        {
            ServerSocket.Bind(LocalEndPoint);
            ServerSocket.Listen(10);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        internal void EndAccept(IAsyncResult ar)
        {
            //create the user, and add them to the userlist
            Client newUser = new Client(this, ServerSocket.EndAccept(ar));
            Clients.TryAdd(newUser.ClientSocket, newUser);

            //start listening on the socket again
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }
    }
}
