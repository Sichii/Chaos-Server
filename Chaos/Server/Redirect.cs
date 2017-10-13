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

using System.Linq;
using System.Net;
using System.Threading;

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
            if (type == ServerType.World)
            {
                Seed = (byte)Utility.Random(0, 9);
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
            EndPoint = new IPEndPoint(new IPAddress(Dns.GetHostEntry(Host.Name).AddressList.FirstOrDefault(ip => ip.GetAddressBytes().Length == 4).GetAddressBytes()), Client.Server.LocalPort);
        }
    }
}
