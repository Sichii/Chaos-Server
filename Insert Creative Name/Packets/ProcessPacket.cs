using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal static class ProcessPacket
    {
        internal static Server Server = null;
        internal static World World = null;
        internal static void CreateCharA(Client client, string name, string password)
        {
            if (name.Length < 4 || name.Length > 12)
                client.Enqueue(ServerPackets.LobbyMessage(3, "Name must be 4-12 letters long."));
            else if (password.Length < 4 || password.Length > 12)
                client.Enqueue(ServerPackets.LobbyMessage(3, "Password must be 4-8 characters long."));
            else if (Regex.Match(name, @"^[A-Za-z]{4, 12}$").Success)
            {
                using (FileStream userHash = File.OpenRead(Paths.UserHash))
                using (BinaryReader reader = new BinaryReader(userHash))
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        if (reader.ReadString().Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            client.Enqueue(ServerPackets.LobbyMessage(3, "Name is taken."));
                            return;
                        }
                        //skips the hash
                        reader.BaseStream.Position += 33;
                    }


                client.NewCharName = name;
                client.NewCharPw = password;
                client.Enqueue(ServerPackets.LobbyMessage(0, "\0"));
            }
            else
                client.Enqueue(ServerPackets.LobbyMessage(3, "Name must consist of only letters."));
        }

        internal static void Login(Client client, string name, string password)
        {
            Objects.User user = World.Objects.Values.OfType<Objects.User>().FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (user != null)
            {
                client.Enqueue(ServerPackets.LobbyMessage(3, "Character is already logged on."));
                user.Client.Disconnect();
                return;
            }

            using (FileStream userHash = File.OpenRead(Paths.UserHash))
            using (BinaryReader reader = new BinaryReader(userHash))
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    if (reader.ReadString().Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (reader.ReadString().Equals(password))
                        {
                            client.User = (Objects.User)Data.Deserialize($@"{Paths.Chars}\{name.ToLower()}");
                            client.User.Resync(client);
                        }
                        else
                            client.Enqueue(ServerPackets.LobbyMessage(3, "Incorrect password."));

                        return;
                    }

                    reader.BaseStream.Position += 33;
                }
        }
    }
}
