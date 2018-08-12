// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;

namespace Chaos
{
    internal sealed class Server
    {
        internal static object Sync = new object();
        internal static int NextClientId = 1000000;
        internal static int NextId = 1;
        internal static bool Running;
        internal static string[] Admins = new string[] { "Sichi", "Jinori", "Vorlof", "JohnGato", "Whug", "Ishikawa", "Legend", "Doms", "Pill", "Styax"};

        internal static readonly object SyncWrite = new object();
        internal static FileStream LogFile;
        internal static StreamWriter LogWriter;
        internal static DateTime Today;

        internal IPEndPoint ClientEndPoint;
        internal IPEndPoint ServerEndPoint;
        internal IPEndPoint LoopbackEndPoint;
        internal Socket ServerSocket { get; set; }

        internal byte[] Table { get; }
        internal uint TableCheckSum { get; }
        internal byte[] LoginMessage { get; }
        internal uint LoginMessageCheckSum { get; }
        private Dictionary<Socket, Client> Clients { get; }
        internal IEnumerable<Client> WorldClients => Clients.Values.Where(client => client.ServerType == ServerType.World && client.User != null);
        internal DataBase DataBase { get; }
        internal GameTime ServerTime => GameTime.Now;
        internal LightLevel LightLevel => ServerTime.TimeOfDay;
        internal List<Redirect> Redirects { get; set; }
        internal List<MetaFile> MetaFiles { get; set; }

        internal Server(IPAddress ip, int port)
        {
            if (!Directory.Exists(Paths.LogFiles))
                Directory.CreateDirectory(Paths.LogFiles);

            LogFile = new FileStream($@"{Paths.LogFiles}{DateTime.UtcNow.ToString("MMM dd yyyy")}.log", FileMode.OpenOrCreate);
            LogWriter = new StreamWriter(LogFile);
            LogWriter.AutoFlush = true;
            Today = DateTime.MinValue;

            WriteLog("Initializing server...");

            ClientEndPoint = new IPEndPoint(ip, port);
            ServerEndPoint = new IPEndPoint(IPAddress.Any, port);
            LoopbackEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            Clients = new Dictionary<Socket, Client>();
            DataBase = new DataBase(this);
            Redirects = new List<Redirect>();
            MetaFiles = new List<MetaFile>();

            byte[] notif = Encoding.GetEncoding(949).GetBytes($@"{{={(char)MessageColor.Orange}Under Construction");
            LoginMessageCheckSum = Crypto.Generate32(notif);

            using (MemoryStream compressor = ZLIB.Compress(notif))
                LoginMessage = compressor.ToArray();

            MemoryStream tableStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(tableStream))
            {
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write(ClientEndPoint.Address.GetAddressBytes());
                writer.Write((byte)(ClientEndPoint.Port / 256));
                writer.Write((byte)(ClientEndPoint.Port % 256));
                writer.Write(Encoding.GetEncoding(949).GetBytes("Chaos;Under Construction\0"));
                writer.Write(notif);
                writer.Flush();

                TableCheckSum = Crypto.Generate32(tableStream.ToArray());
                using (MemoryStream table = ZLIB.Compress(tableStream.ToArray()))
                    Table = table.ToArray();
            }

            foreach (string name in Directory.GetFiles(Paths.MetaFiles))
                MetaFiles.Add(MetaFile.Load(new FileInfo(name).Name));
        }

        ~Server()
        {
            Running = false;
            LogFile.Dispose();
        }

        internal void ServerLoop()
        {
            Running = true;
            Game.Set(this);

            //display dns ip for others to connect to
            WriteLog($"Server IP: {ClientEndPoint.Address}");
            WriteLog("Starting the serverloop...");

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(ServerEndPoint);
            ServerSocket.Listen(10);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), ServerSocket);

            while (Running)
            {
                lock (Sync)
                {
                    foreach (Client client in Clients.Values)
                    {
                        if (!client.Connected)
                        {
                            client.Disconnect();
                            continue;
                        }

                        lock (client.SendQueue)
                        {
                            while (client.SendQueue.Count > 0)
                            {
                                ServerPacket serverPacket = client.SendQueue.Dequeue();
                                if (serverPacket == null) return;
                                WriteLog(serverPacket.ToString(), client);

                                if (serverPacket.IsEncrypted)
                                {
                                    serverPacket.Counter = client.Signature++;
                                    serverPacket.Encrypt(client.Crypto);
                                }

                                if (serverPacket.OpCode == 98)
                                    client.Signature = serverPacket.Counter;

                                client.Send(serverPacket);
                            }
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }

        internal bool TryAddClient(Client client)
        {
            lock (Sync)
            {
                if (!Clients.ContainsKey(client.ClientSocket))
                {
                    Clients.Add(client.ClientSocket, client);
                    return true;
                }
            }
            return false;
        }

        internal bool TryRemoveClient(Client client)
        {
            lock (Sync)
            {
                if (Clients.ContainsKey(client.ClientSocket))
                {
                    Clients.Remove(client.ClientSocket);
                    return true;
                }
            }

            return false;
        }

        internal void EndAccept(IAsyncResult ar)
        {
            Socket clientSocket = ServerSocket.EndAccept(ar);
            ServerSocket.BeginAccept(new AsyncCallback(EndAccept), ServerSocket);

            if (clientSocket.Connected)
            {
                //create the user, and add them to the userlist
                Client newClient = new Client(this, clientSocket);

                WriteLog($@"Incoming connection", newClient);

                newClient.Connect();
            }
        }

        internal static void WriteLog(string message, Client client = null)
        {
            lock (SyncWrite)
            {
                if (client == null)
                    message = $@"[{DateTime.UtcNow.ToString("HH:mm")}] Server: {message}";
                else
                    message = $@"[{DateTime.UtcNow.ToString("HH:mm")}] {client.IpAddress}: {message}";

                Console.WriteLine(message);

                if (DateTime.UtcNow.Day != Today.Day)
                {
                    if (!File.Exists($@"{Paths.LogFiles}{DateTime.UtcNow.ToString("MMM dd yyyy")}.log"))
                    {
                        LogFile.Dispose();
                        LogFile = new FileStream($@"{Paths.LogFiles}{DateTime.UtcNow.ToString("MMM dd yyyy")}.log", FileMode.OpenOrCreate);
                        LogWriter = new StreamWriter(LogFile);
                    }
                    Today = DateTime.UtcNow;
                }

                LogWriter.Write($@"{message}{Environment.NewLine}");
            }
        }

        internal bool TryGetUser(string name, out User user) => (user = WorldClients.FirstOrDefault(client => client?.User?.Name?.Equals(name, StringComparison.CurrentCultureIgnoreCase) == true)?.User) != null;
        internal bool TryGetUser(int id, out User user) => (user = WorldClients.FirstOrDefault(client => client?.User?.Id == id)?.User) != null;
    }
}
