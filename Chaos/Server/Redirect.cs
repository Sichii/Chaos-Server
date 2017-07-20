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
        internal string Name { get; }
        internal byte[] Key { get; }
        internal byte Seed { get; }
        internal ServerType Type { get; }

        internal Redirect(Client client, ServerType type, string name = null)
        {
            Id = Interlocked.Increment(ref Server.NextId);
            Client = client;
            Type = type;
            Name = name ?? "Lobby";

            /*
            if (type != ServerType.Lobby)
            {
                //Seed = (byte)Utility.Random(0, 9);
                Seed = client.Crypto.Seed;
                
                List<byte> key = new List<byte>();
                for (int i = 0; i < 9; i++)
                    key.Add((byte)Utility.Random(44, 68));

                Key = key.ToArray();
            }
            else
            {
                Seed = client.Crypto.Seed;
                Key = client.Crypto.Key;
            }
            */
            
            
            Seed = client.Crypto.Seed;
            Key = client.Crypto.Key;
            

            EndPoint = new IPEndPoint(Dns.GetHostEntry(Paths.DynHost).AddressList[0], Client.Server.LocalPort);
        }
    }
}
