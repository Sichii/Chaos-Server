using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Chaos
{
    internal sealed class WorldServer
    {
        internal static readonly object SyncObj = new object();
        private IPAddress LocalIp;
        private int LocalPort;
        private IPEndPoint LocalEndPoint;
        internal Socket ServerSocket;
        internal ClientPacketHandler[] ClientPacketHandlers { get; }
        internal World World { get; }
        internal byte[] Table { get; }

        internal WorldServer(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientPacketHandlers = new ClientPackets().Handlers;
            World = new World();
            ProcessPacket.Server = this;
            ProcessPacket.World = World;
            Table = Properties.Resources.mServer;
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
            Client newClient = new Client(this, ServerSocket.EndAccept(ar));
            World.Clients.TryAdd(newClient.ClientSocket, newClient);

            //start listening on the socket again
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }
    }
}
