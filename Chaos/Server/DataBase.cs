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
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Chaos
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class DataBase
    {
        private Server Server { get; }
        private string HashKey => Crypto.GetMD5Hash("UserHash");
        private const string MapKey = "edl396yhvnw85b6kd8vnsj296hj285bq";
        private Dictionary<string, string> UserHash => Cache.Get<Dictionary<string, string>>(HashKey);
        private NewtonsoftSerializer Serializer { get; }
        private StackExchangeRedisCacheClient Cache { get; }
        private ConnectionMultiplexer DataConnection { get; }

        internal byte[] MapData => Cache.Get<byte[]>(MapKey);

        internal DataBase(Server server)
        {
            Server.WriteLog("Creating the database connection...");

            Server = server;
            //create the serializing cache db
            JsonSerializerSettings jSettings = new JsonSerializerSettings();
            jSettings.TypeNameHandling = TypeNameHandling.All;
            jSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Serializer = new NewtonsoftSerializer(jSettings);
            DataConnection = ConnectionMultiplexer.Connect(Paths.RedisConfig);
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            //keep the db clear for now (except the map file)
            foreach (var key in DataConnection.GetServer(Paths.RedisConfig).Keys())
                if (key != MapKey)
                    Cache.Remove(key);    

            if (!Cache.Exists(HashKey))
                Cache.Add(HashKey, new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Removes a user from the UserHash and the Cache
        /// </summary>
        /// <param name="key">Name of the user to remove</param>
        internal void RemoveUser(string key)
        {
            string hash;
            ConcurrentDictionary<string, string> userHash = new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var kvp in UserHash)
                userHash.TryAdd(kvp.Key, kvp.Value);

            if(userHash.TryRemove(key, out hash))
                Cache.Replace(HashKey, userHash);

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
        internal bool CheckHash(string name, string hash) => UserHash.Any(kvp => kvp.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase) && kvp.Value.Equals(hash));

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

            return  Cache.Add(user.Name.ToUpper(), user) && Cache.Replace(HashKey, userHash);
        }

        /// <summary>
        /// Changes the password hash of the given username
        /// </summary>
        /// <param name="name">Name of user to change password for.</param>
        /// <param name="oldPw">User's current password.</param>
        /// <param name="newPw">Password to change to.</param>
        internal bool ChangePassword(string name, string currentPw, string newPw)
        {
            if (CheckHash(name, currentPw))
            {
                Dictionary<string, string> userHash = UserHash;
                userHash[name] = Crypto.GetMD5Hash(newPw);
                if (Cache.Replace(HashKey, userHash))
                    return true;
            }
            return false;
        }

        ~DataBase()
        {
            Cache.Dispose();
            DataConnection.Dispose();
        }
    }
}
