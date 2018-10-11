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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Immutable;
namespace Chaos
{
    // *************************************************************************************************
    // A substantial amount of this file was taken and rewritten from a project called "ProxyBase"
    // The code that this file is based on was written by Kyle Speck (kojasou) <kojasou@hybrasyl.com> <https://github.com/kojasou>
    // The current owner of the original code is The Hy-Brasyl Project. <https://www.hybrasyl.com/>
    // *************************************************************************************************
    /// <summary>
    /// Object containing methods and objects used for server encryption.
    /// </summary>
    public sealed class Crypto
    {
        internal byte Seed { get; set; }
        internal byte[] Key { get; set; }
        internal byte[] KeySalts { get; set; }

        internal ImmutableArray<byte> Salts => CONSTANTS.SALT_TABLE[Seed];

        internal Crypto() : this(0, Encoding.ASCII.GetBytes("UrkcnItnI"), string.Empty) { }
        internal Crypto(byte seed, byte[] key, string keySaltSeed)
        {
            Seed = seed;
            Key = key;
            KeySalts = string.IsNullOrEmpty(keySaltSeed) ? new byte[1024] : GenerateKeySalts(keySaltSeed);
        }

        internal byte[] GenerateKey(ushort a, byte b)
        {
            byte[] key = new byte[9];
            for (int i = 0; i < 9; i++)
                key[i] = KeySalts[((i * ((9 * i) + (b * b))) + a) % KeySalts.Length];

            return key;
        }
        internal static byte[] GenerateKeySalts(string seed)
        {
            string saltTable = GetMD5Hash(GetMD5Hash(seed));
            for (int i = 0; i < 31; i++)
                saltTable += GetMD5Hash(saltTable);

            return Encoding.ASCII.GetBytes(saltTable);
        }
        internal static ushort Generate16(byte[] data) => Generate16(data, 0, data.Length);
        internal static ushort Generate16(byte[] data, int index, int length)
        {
            uint checkSum = 0;
            for (int i = 0; i < length; ++i)
                checkSum = (ushort)(data[index + i] ^ ((checkSum << 8) ^ CONSTANTS.TABLE16[(int)(checkSum >> 8)]));
            return (ushort)checkSum;
        }
        internal static uint Generate32(byte[] data)
        {
            uint checkSum = uint.MaxValue;
            for (int i = 0; i < data.Length; ++i)
                checkSum = (checkSum >> 8) ^ CONSTANTS.TABLE32[(int)((checkSum & byte.MaxValue) ^ data[i])];
            return checkSum;
        }
        public static string GetMD5Hash(string value) => BitConverter.ToString(HashAlgorithm.Create("MD5").ComputeHash(Encoding.ASCII.GetBytes(value))).Replace("-", string.Empty).ToLower();

        public static void EncryptFile(MemoryStream fileData, string path)
        {
            //generate a random cipherkey
            byte[] cipherKey = new byte[32];

            for(int i = 0; i < 32; i++)
                cipherKey[i] = (byte)Utilities.Random(0, byte.MaxValue, true);

            var AES = new AesCryptoServiceProvider()
            {
                BlockSize = 256,
                KeySize = 256,
                Key = cipherKey,
                IV = Encoding.UTF8.GetBytes(GetMD5Hash(Encoding.UTF8.GetString(cipherKey)).Substring(30, 34))
            };


            FileStream file = File.Create(path);

            ICryptoTransform encryptor = AES.CreateEncryptor();
            using (var crypt = new CryptoStream(file, encryptor, CryptoStreamMode.Write))
            {
                byte[] data = fileData.ToArray();
                crypt.Write(data, 0, data.Length);
            }

            //insert cipher at random location, place index to cipher at the beginning of the file.
            byte[] newData = File.ReadAllBytes(path);
            string dataStr = Encoding.Unicode.GetString(newData);
            byte pos = (byte)Utilities.Random(1, dataStr.Length - 1);
            dataStr.Insert(pos, Encoding.UTF8.GetString(cipherKey));
            dataStr.Insert(0, Encoding.UTF8.GetString(new byte[] { pos }));
            //rewrite the file
            File.WriteAllBytes(path, Encoding.Unicode.GetBytes(dataStr));
        }
        public static MemoryStream DecryptFile(string path)
        {
            //read the file
            byte[] fileData = File.ReadAllBytes(path);
            string dataStr = Encoding.Unicode.GetString(fileData);
            //get the first byte as the position to the cipherkey
            byte pos = Encoding.UTF8.GetBytes(new char[] { dataStr[0] })[0];
            //read the cipher from the indexed location
            byte[] cipherKey = Encoding.UTF8.GetBytes(dataStr.Substring(pos + 1, 32));
            //generate the symmetric algorithm key from the cipherkey
            byte[] symmetric = Encoding.UTF8.GetBytes(GetMD5Hash(Encoding.UTF8.GetString(cipherKey)).Substring(30, 34));
            //remove the cipherkey and index from the data
            byte[] newData = Encoding.Unicode.GetBytes(dataStr.Substring(1, pos) + dataStr.Substring(pos + 1 + 32));

            //decrypt
            var AES = new AesCryptoServiceProvider()
            {
                BlockSize = 256,
                KeySize = 256,
                Key = cipherKey,
                IV = symmetric
            };

            var dataStream = new MemoryStream(newData);
            ICryptoTransform decryptor = AES.CreateDecryptor();
            var crypt = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Read);

            using (var reader = new StreamReader(crypt))
                return new MemoryStream(Encoding.Unicode.GetBytes(reader.ReadToEnd()));
        }
    }
}