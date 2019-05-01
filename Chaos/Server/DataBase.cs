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
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Chaos
{
    /// <summary>
    /// The interface between the redis database and the server.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal sealed class DataBase
    {
        private readonly Server Server;
        private readonly NewtonsoftSerializer Serializer;
        private readonly StackExchangeRedisCacheClient Cache;
        private readonly ConnectionMultiplexer DataConnection;

        private Dictionary<string, string> UserHash => Cache.Get<Dictionary<string, string>>(UserHashKey);
        private string UserHashKey => Crypto.GetMD5Hash("UserHash") + Crypto.GetMD5Hash("ServerObjSuffix");
        private string MapKey => Crypto.GetMD5Hash("Maps") + Crypto.GetMD5Hash("ServerObjSuffix");
        private string GuildKey => Crypto.GetMD5Hash("Guilds") + Crypto.GetMD5Hash("ServerObjSuffix");

        internal byte[] MapData => Cache.Get<byte[]>(MapKey);
        internal byte[] GuildData => Cache.Get<byte[]>(GuildKey);

        internal DataBase(Server server)
        {
            Server.WriteLogAsync("Creating the database connection...");

            Server = server;
            //create the serializing cache db
            var jSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            Serializer = new NewtonsoftSerializer(jSettings);
            DataConnection = ConnectionMultiplexer.Connect(Paths.RedisConfig);
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            //keep the db clear for now (except the map file)
            foreach (RedisKey key in DataConnection.GetServer(Paths.RedisConfig).Keys())
                if (key != MapKey)
                    Cache.Remove(key);    

            if (!Cache.Exists(UserHashKey))
                Cache.Add(UserHashKey, new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase));
            if (!Cache.Exists(GuildKey))
                Cache.Add(GuildKey, new byte[4] { 0, 0, 0, 0 });
        }

        /// <summary>
        /// Attempts to add a new user to the database. Returns false if it fails for any reason.
        /// </summary>
        /// <param name="user">Name of the user you'd like to add.</param>
        /// <param name="password">User's password unhashed.</param>
        internal bool TryAddUser(User user, string password)
        {
            string hash = Crypto.GetMD5Hash(password);
            Dictionary<string, string> userHash = UserHash;

            if (userHash.ContainsKey(user.Name))
                return false;

            userHash.Add(user.Name, hash);

            return Cache.Add(user.Name.ToUpper(), user) && Cache.Replace(UserHashKey, userHash);
        }

        /// <summary>
        /// Removes a user from the UserHash and the Cache
        /// </summary>
        /// <param name="key">Name of the user to remove</param>
        internal void RemoveUser(string key)
        {
            var userHash = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (KeyValuePair<string, string> kvp in UserHash)
                userHash.Add(kvp.Key, kvp.Value);

            if(userHash.Remove(key))
                Cache.Replace(UserHashKey, userHash);

            Cache.Remove(key);
        }

        /// <summary>
        /// Checks the UserHash file to see if the character exists
        /// </summary>
        /// <param name="name">Name of the user you wish to check on.</param>
        internal bool UserExists(string name) => UserHash.Any(kvp => kvp.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Attempts save the given user if it exists. Returns false if the user doesn't exist.
        /// </summary>
        /// <param name="user">User you'd like to save.</param>
        internal bool TrySaveUser(User user) => Cache.Replace(user.Name.ToUpper(), user);

        /// <summary>
        /// Retreives the user corresponding to the name given. Returns null if the user doesn't exist.
        /// </summary>
        /// <param name="name">Name of the user you'd like to retreive.</param>
        internal User GetUser(string name) => UserExists(name) ? Cache.Get<User>(name.ToUpper()) : null;

        /// <summary>
        /// Checks the given name and hash pair in the UserHash. Returns true if they match.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="hash">Hash of the user's password.</param>
        internal bool CheckPassword(string name, string password) => UserHash.Any(kvp => kvp.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase) && kvp.Value.Equals(Crypto.GetMD5Hash(password)));

        /// <summary>
        /// Changes the password hash of the given username
        /// </summary>
        /// <param name="name">Name of user to change password for.</param>
        /// <param name="oldPw">User's current password.</param>
        /// <param name="newPw">Password to change to.</param>
        internal bool ChangePassword(string name, string currentPw, string newPw)
        {
            if (CheckPassword(name, currentPw))
            {
                Dictionary<string, string> userHash = UserHash;
                userHash[name] = Crypto.GetMD5Hash(newPw);
                if (Cache.Replace(UserHashKey, userHash))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the guild data to the database.
        /// </summary>
        /// <param name="guildData">The guild data, written to a byte array from a stream.</param>
        internal bool TrySaveGuilds(byte[] guildData) => Cache.Replace(GuildKey, guildData);

        ~DataBase()
        {
            Cache.Dispose();
            DataConnection.Dispose();
        }
    }
}
