using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace Chaos
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        internal static readonly object SyncWrite = new object();
        internal static int NextId = 1;
        private IPAddress LocalIp;
        private int LocalPort;
        private IPEndPoint LocalEndPoint;
        internal Socket ServerSocket { get; set; }
        internal ServerPackets Packets { get; }
        internal World World { get; }
        internal byte[] Table { get; }
        internal ConcurrentDictionary<Socket, Client> Clients { get; }
        internal List<Client> WorldClients => Clients.Values.Where(c => c.ServerType == ServerType.World).ToList();
        internal DataBase Cache { get; }

        internal Server(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            World = new World(this);
            Clients = new ConcurrentDictionary<Socket, Client>();
            ProcessPacket.Server = this;
            ProcessPacket.World = World;
            Table = Properties.Resources.mServer;
            Packets = new ServerPackets();
            Cache = new DataBase();
        }

        internal void Start()
        {
            World.Load();
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(LocalEndPoint);
            ServerSocket.Listen(100);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        internal void EndAccept(IAsyncResult ar)
        {
            //create the user, and add them to the userlist
            Client newClient = new Client(this, ServerSocket.EndAccept(ar));
            if (Clients.TryAdd(newClient.ClientSocket, newClient))
                newClient.Connect();

            //start listening on the socket again
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        internal void WriteLog(string message, Client client = null)
        {
            lock (SyncWrite)
            {
                if (client == null)
                    message = $@"[{DateTime.Now.ToString("HH:mm")}] Server: {message}";
                else
                    message = $@"[{DateTime.Now.ToString("HH:mm")}] {(client.ClientSocket.RemoteEndPoint as IPEndPoint).Address}: {message}";

                using (StreamWriter writer = File.AppendText(Paths.LogFiles))
                    writer.Write(message);
            }
        }
    }
}
