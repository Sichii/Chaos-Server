using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chaos
{
    internal class Redirect
    {
        internal int Id { get; }
        internal IPEndPoint EndPoint { get; }
        internal Client Client { get; }
        internal Server Server { get; }
        internal string Name { get; }
        internal byte[] Key { get; }
        internal byte Seed { get; }
        internal ServerType Type { get; }

        internal Redirect(Client client, Server server, ServerType type, string name, byte seed, byte[] key)
        {
            Id = Interlocked.Increment(ref Server.NextId);
            Client = client;
            Server = server;
            Type = type;
            Name = name;
            Seed = seed;
            Key = key;
            EndPoint = server.ServerSocket.LocalEndPoint as IPEndPoint;
        }
    }
}
