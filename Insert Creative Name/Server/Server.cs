using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Chaos
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        private IPAddress LocalIp;
        private int LocalPort;
        private IPEndPoint LocalEndPoint;
        internal Socket ServerSocket;
        internal ClientPacketHandler[] ClientPacketHandlers { get; }
        internal ConcurrentDictionary<Socket, Client> LoginClients { get; }
        internal ConcurrentDictionary<Socket, Client> LobbyClients { get; }
        internal World World { get; }
        internal byte[] Table { get; }

        internal Server(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientPacketHandlers = new ClientPackets().Handlers;
            World = new World();
            LoginClients = new ConcurrentDictionary<Socket, Client>();
            LobbyClients = new ConcurrentDictionary<Socket, Client>();
            ProcessPacket.Server = this;
            ProcessPacket.World = World;
            Table = Properties.Resources.mServer;
        }

        internal void Start()
        {
            ServerSocket.Bind(LocalEndPoint);
            ServerSocket.Listen(100);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        internal void EndAccept(IAsyncResult ar)
        {
            //create the user, and add them to the userlist
            Client newClient = new Client(this, ServerSocket.EndAccept(ar));
            if (LoginClients.TryAdd(newClient.ClientSocket, newClient))
                newClient.Connect();

            //start listening on the socket again
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }
    }
}
