using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chaos
{
    internal sealed class Client
    {
        private static readonly object ClientSync = new object();
        private bool Connected = false;
        private byte[] ClientBuffer = new byte[4096];
        private List<byte> FullClientBuffer = new List<byte>();
        private Queue<ServerPacket> SendQueue = new Queue<ServerPacket>();
        private Queue<ClientPacket> ProcessQueue = new Queue<ClientPacket>();
        private byte ServerSequence = 0;
        private ClientPacketHandler[] PacketHandlers { get; }
        private Thread ClientThread { get; }
        internal IPAddress IpAddress { get; }
        internal ServerType ServerType { get; set; }
        internal Server Server { get; }
        internal Socket ClientSocket { get; }
        internal Crypto Crypto { get; set; }
        internal Objects.User User { get; set; }
        internal string NewCharName { get; set; }
        internal string NewCharPw { get; set; }
        internal int StepCount;
        internal int Id { get; }
        internal DateTime LastClickObj { get; set; }

        /// <summary>
        /// Creates a new user with reference to the server, and the user's socket.
        /// </summary>
        /// <param name="server">The game server.</param>
        /// <param name="socket">The user's socket.</param>
        internal Client(Server server, Socket socket)
        {
            Server = server;
            ServerType = ServerType.Lobby;
            ClientSocket = socket;
            Crypto = new Crypto();
            PacketHandlers = new ClientPackets().Handlers;
            StepCount = 1;
            Id = Interlocked.Increment(ref Server.NextClientId);
            LastClickObj = DateTime.MinValue;
            IpAddress = (socket.RemoteEndPoint as IPEndPoint).Address;
            ClientThread = new Thread(ClientLoop);
        }

        /// <summary>
        /// Connects to the socket and begins receiving data.
        /// </summary>
        internal void Connect()
        {
            Connected = true;
            ClientThread.Start();
            if (ServerType != ServerType.World)
                Enqueue(Server.Packets.AcceptConnection());

            Server.WriteLog($@"Connection accepted", this);
            //when we receive data, copy the readable data to the client buffer and call endreceive
            ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), null);
        }

        /// <summary>
        /// Disconnects the user from the server.
        /// </summary>
        /// <param name="wait">False if you want to immediately kill the client. True if you want the client to time out.</param>
        internal void Disconnect(bool wait = false)
        {
            Connected = false;
            if (wait)
                return;

            Client dis = this;
            if (Server.Clients.TryRemove(ClientSocket, out dis))
            {
                if (User != null)
                    try { Server.World.ScrubUser(User); }
                    catch { }
                ClientSocket.Disconnect(false);
            }
        }

        /// <summary>
        /// Asynchronous operation to receive information from the client
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void ClientEndReceive(IAsyncResult ar)
        {
            lock (ClientSync)
            {
                //get the length of the packet
                int length = ClientSocket.EndReceive(ar);
                //if we receive a packet with no length, disconnect the client, something went wrong
                if (length == 0)
                    Disconnect();
                else
                {
                    //otherwise copy the client buffer into a new byte array sized to fit the length of the packet
                    byte[] numArray = new byte[length];
                    Array.Copy(ClientBuffer, numArray, length);
                    //copy that array into the full client buffer, so we can deal with the information in a properly sized list
                    FullClientBuffer.AddRange(numArray);
                    while (FullClientBuffer.Count > 3)
                    {
                        //check to see if it's a valid packet, this gives us the number of bytes that arent trailing random shit
                        int count = (FullClientBuffer[1] << 8) + FullClientBuffer[2] + 3;
                        if (count <= FullClientBuffer.Count)
                        {
                            //copy the non-random shit into a new list, remove it from the client buffer
                            List<byte> range = FullClientBuffer.GetRange(0, count);
                            FullClientBuffer.RemoveRange(0, count);
                            //create a clientpacket out of the readable data, and send it to be processed by the server
                            ClientPacket clientPacket = new ClientPacket(range.ToArray());
                            lock (ProcessQueue)
                                ProcessQueue.Enqueue(clientPacket);
                        }
                        else
                            break;
                    }
                }
            }
            if (Connected) //begin checking for received info again
                ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), null);
        }

        /// <summary>
        /// Asynchronous operation to end a pending send.
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void EndSend(IAsyncResult ar) => ((Socket)ar.AsyncState).EndSend(ar);

        /// <summary>
        /// Sends packets to the process/send thread.
        /// </summary>
        /// <param name="packets">Packet(s) to be sent.</param>
        internal void Enqueue(params ServerPacket[] packets)
        {
            lock (SendQueue)
            {
                for (int i = 0; i < packets.Length; i++)
                    SendQueue.Enqueue(packets[i]);
            }
        }

        /// <summary>
        /// Thread/Loop where packets from the client are processed, and the server sends packets back to the client.
        /// </summary>
        private void ClientLoop()
        {
            while (Connected)
            {
                lock (SendQueue)//lock to send server packets
                {
                    while (SendQueue.Count > 0)//while there are packets to send
                    {
                        ServerPacket packet = SendQueue.Dequeue();//get the next packet in the queue, convert to serverpacket
                        Server.WriteLog(packet.ToString(), this);
                        if (packet == null) continue;

                        if (packet.ShouldEncrypt)//if it should be encrypted, do it
                        {
                            packet.Sequence = ServerSequence++;
                            packet.Encrypt(Crypto);
                        }
                        
                        byte[] data = packet.ToArray();//get the packet's data and try to send it
                        try { ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), ClientSocket); }
                        catch { }
                    }
                }
                lock (ProcessQueue)//lock to receive client packets
                {
                    while (ProcessQueue.Count > 0)//while there are packets to process
                    {
                        ClientPacket packet = ProcessQueue.Dequeue();//get the next packet in the queue, convert to clientpacket
                        if (packet == null) continue;

                        if (packet.ShouldEncrypt)//if it is encrypted, decrypt it
                            packet.Decrypt(Crypto);
                        
                        if (packet.IsDialog)//if packet is a dialog, decrypt the header
                            packet.DecryptDialog();
                        Server.WriteLog(packet.ToString(), this);
                        ClientPacketHandler handle = PacketHandlers[packet.OpCode];//get the handler for this packet
                        if (handle != null)//if we have a handler for this packet
                            try { handle(this, packet); } //no srsly, please handle it
                            catch { }
                    }
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Changes updates the location of the client in the path to logging in.
        /// </summary>
        /// <param name="redirect">Redirect information.</param>
        internal void Redirect(Redirect redirect)
        {
            Server.Redirects.Add(redirect);
            Enqueue(Server.Packets.Redirect(redirect));
        }
    }
}
