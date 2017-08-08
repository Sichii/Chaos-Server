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
        private Server Server { get; }
        private string HashKey => Crypto.GetHashString("UserHash", "MD5");
        private ConcurrentDictionary<string, string> UserHash => Cache.Get<ConcurrentDictionary<string, string>>(HashKey);
        internal NewtonsoftSerializer Serializer { get; }
        internal StackExchangeRedisCacheClient Cache { get; }
        internal ConnectionMultiplexer DataConnection { get; }

        internal DataBase(Server server)
        {
            Server = server;
            //create the serializing cache db
            JsonSerializerSettings jSettings = new JsonSerializerSettings();
            jSettings.TypeNameHandling = TypeNameHandling.All;
            jSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Serializer = new NewtonsoftSerializer(jSettings);
            ConfigurationOptions config = new ConfigurationOptions();
            DataConnection = ConnectionMultiplexer.Connect("localhost:6379");
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            if (!Cache.Exists(HashKey))
                Cache.Add(HashKey, new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase));

            RemoveUser("sichi");
            RemoveUser("vorlof");
            RemoveUser("bivins");
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
        internal User GetUser(string name)
        {
            User u = Cache.Get<User>(name.ToUpper());
            return UserExists(name) ? Cache.Get<User>(name.ToUpper()) : null;
        }

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
            string hash = Crypto.GetHashString(password, "MD5");
            ConcurrentDictionary<string, string> userHash = UserHash;

            if (userHash.TryAdd(user.Name, hash))
                if (Cache.Add(user.Name.ToUpper(), user))
                {
                    Cache.Replace(HashKey, userHash);
                    return true;
                }
                else
                    UserHash.TryRemove(user.Name, out hash);

            return false;
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
                ConcurrentDictionary<string, string> userHash = UserHash;
                userHash[name] = Crypto.GetHashString(newPw, "MD5");
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
