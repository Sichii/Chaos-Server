using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core;
using System.Collections.Concurrent;

namespace Chaos
{
    internal class DataBase
    {
        private string HashKey => Crypto.GetHashString("UserHash", "MD5");
        private ConcurrentDictionary<string, string> UserHash => Cache.Get<ConcurrentDictionary<string, string>>(HashKey);
        internal NewtonsoftSerializer Serializer { get; }
        internal StackExchangeRedisCacheClient Cache { get; }
        internal ConnectionMultiplexer DataConnection { get; }

        internal DataBase()
        {
            //create the serializing cache db
            JsonSerializerSettings jSettings = new JsonSerializerSettings();
            jSettings.TypeNameHandling = TypeNameHandling.All;
            Serializer = new NewtonsoftSerializer(jSettings);
            DataConnection = ConnectionMultiplexer.Connect("ChaosDB");
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            if (!Cache.Exists(HashKey))
                Cache.Add(HashKey, new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase));

        }

        /// <summary>
        /// Checks the UserHash file to see if the character exists
        /// </summary>
        /// <param name="name">Name of the user you wish to check on.</param>
        internal bool UserExists(string name) => UserHash.ContainsKey(name);
        /// <summary>
        /// Attempts save the given user if it exists. Returns false if the user doesn't exist.
        /// </summary>
        /// <param name="user">User you'd like to save.</param>
        internal bool TrySaveUser(Objects.User user) => Cache.Replace(user.Name.ToUpper(), user);
        /// <summary>
        /// Retreives the user corresponding to the name given. Returns null if the user doesn't exist.
        /// </summary>
        /// <param name="name">Name of the user you'd like to retreive.</param>
        internal Objects.User GetUser(string name) => UserExists(name) ? Cache.Get<Objects.User>(name.ToUpper()) : null;
        /// <summary>
        /// Checks the given name and hash pair in the UserHash. Returns true if they match.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="hash">Hash of the user's password.</param>
        internal bool CheckHash(string name, string hash) => UserHash[name] == hash;
        /// <summary>
        /// Attempts to add a new user to the database. Returns false if it fails for any reason.
        /// </summary>
        /// <param name="user">Name of the user you'd like to add.</param>
        /// <param name="password">User's password unhashed.</param>
        internal bool TryAddUser(Objects.User user, string password)
        {
            string hash = Crypto.GetHashString(password, "MD5");
            if (UserHash.TryAdd(user.Name, password))
                if (Cache.Add(user.Name.ToUpper(), user))
                    return true;
                else
                    UserHash.TryRemove(user.Name, out hash);

            return false;
        }
        /// <summary>
        /// Tells the database to perform a worldsave in the background.
        /// </summary>
        internal void Save() => Cache.Save(SaveType.BackgroundSave);

        ~DataBase()
        {
            Cache.Save(SaveType.ForegroundSave);
            Cache.Dispose();
            DataConnection.Dispose();
        }
    }
}
