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

using System.Net;
using System.Text;
using System.Threading;

namespace Chaos
{
    /// <summary>
    /// Represents a transfer of connection, and change of encryption for a client.
    /// </summary>
    internal sealed class Redirect
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
            Id = Interlocked.Increment(ref Server.NextID);
            Client = client;
            Type = type;
            Name = name ?? "Login";

            if (type == ServerType.World)
            {
                //Seed = (byte)Utility.Random(0, 9);
                //Seed = client.Crypto.Seed;

                /*
                byte[] key = new byte[9];
                for (int i = 0; i < 9; i++)
                {
                    int x = Utilities.Random(1, 10);
                    if (x > 5)
                        key[i] = (byte)Utilities.Random(65, 90);
                    else
                        key[i] = (byte)Utilities.Random(97, 122);
                }

                Key = key;
                */
            }
            else
            {
                Seed = client.Crypto.Seed;
                Key = client.Crypto.Key;
            }

            Seed = client.Crypto.Seed;
            Key = client.Crypto.Key;
            EndPoint = new IPEndPoint(client.IsLoopback ? IPAddress.Loopback : client.Server.ServerEndPoint.Address, Server.GetPort(type));
        }

        public override string ToString() => $@"ID: {Id} | SERVER_TYPE: {Type} | END_POINT: {EndPoint} | NAME: {Name} | SEED: {Seed} | KEY: {Encoding.ASCII.GetString(Key)}";
    }
}
