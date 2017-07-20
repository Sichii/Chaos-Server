using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

namespace Chaos
{
    internal sealed class Server
    {
        internal static readonly object SyncObj = new object();
        internal static readonly object SyncWrite = new object();
        internal static int NextClientId = 10000;
        internal static int NextId = 1;
        internal IPAddress LocalIp;
        internal IPEndPoint LocalEndPoint;
        internal int LocalPort;
        internal Socket ServerSocket { get; set; }
        internal ServerPackets Packets { get; }
        internal World World { get; }
        internal byte[] Table { get; }
        internal uint TableCRC { get; }
        internal byte[] Notification { get; }
        internal uint NotificationCRC { get; }
        internal ConcurrentDictionary<Socket, Client> Clients { get; }
        internal List<Client> WorldClients => Clients.Values.Where(c => c.ServerType == ServerType.World).ToList();
        internal DataBase DataBase { get; }
        internal GameTime GameTime => GameTime.Now;
        internal LightLevel LightLevel => GameTime.TimeOfDay;
        internal List<Redirect> Redirects { get; set; }

        internal Server(IPAddress ip, int port)
        {
            LocalIp = ip;
            LocalPort = port;
            LocalEndPoint = new IPEndPoint(LocalIp, LocalPort);
            World = new World(this);
            Clients = new ConcurrentDictionary<Socket, Client>();
            ProcessPacket.Server = this;
            ProcessPacket.World = World;
            Packets = new ServerPackets();
            DataBase = new DataBase(this);
            Redirects = new List<Redirect>();

            byte[] notif = Encoding.GetEncoding(949).GetBytes($@"{{={(char)MessageColor.Orange}Under Construction");
            NotificationCRC = CRC32.Calculate(notif);

            using (MemoryStream compressor = ZLIB.Compress(notif))
                Notification = compressor.ToArray();

            using (MemoryStream tableStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(tableStream))
            {
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write(Dns.GetHostEntry(Host.Name).AddressList[0].GetAddressBytes());
                writer.Write((byte)(LocalPort / 256));
                writer.Write((byte)(LocalPort % 256));
                writer.Write(Encoding.GetEncoding(949).GetBytes("Chaos\0"));
                writer.Write(notif);

                TableCRC = CRC32.Calculate(tableStream.ToArray());
                using (MemoryStream table = ZLIB.Compress(tableStream.ToArray()))
                    Table = table.ToArray();

            }
        }

        internal void Start()
        {
            World.Load();
            WriteLog("Loading completed.");
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(LocalEndPoint);
            ServerSocket.Listen(100);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        internal void EndAccept(IAsyncResult ar)
        {
            //create the user, and add them to the userlist
            Client newClient = new Client(this, ServerSocket.EndAccept(ar));
            WriteLog($@"Incomming connection", newClient);

            if (Clients.TryAdd(newClient.ClientSocket, newClient))
                newClient.Connect();

            //accept more connections on the socket again
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

                Console.WriteLine(message);
                using (StreamWriter writer = File.AppendText($@"{Paths.LogFiles}{DateTime.UtcNow.ToString("MMM dd yyyy")}.log"))
                    writer.Write($@"{message}{Environment.NewLine}");
            }
        }

        internal bool TryGetUser(string name, out Objects.User user) => (user = Clients.Values.FirstOrDefault(client => client.ServerType == ServerType.World && client.User?.Name?.Equals(name, StringComparison.CurrentCultureIgnoreCase) == true)?.User) != null;
    }
}
