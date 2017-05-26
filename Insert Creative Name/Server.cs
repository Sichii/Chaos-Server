using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Insert_Creative_Name
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        private IPAddress LocalIp;
        private int LocalPort; //25252
        private IPEndPoint LocalEndPoint;
        internal Socket ServerSocket;
        internal ClientPacketHandler[] ClientPacketHandlers { get; }
        internal World World { get; }

        internal Server(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientPacketHandlers = new ClientPackets().Handlers;
            World = new World();
            ProcessPacket.Server = this;
            ProcessPacket.World = World;
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
            World.Clients.TryAdd(newUser.ClientSocket, newUser);

            //start listening on the socket again
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }
    }
}
