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
using System.Linq;

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

        public static void EncryptFile(MemoryStream rawStream, string path)
        {
            //generate a random cipherkey
            byte[] cipherKey = new byte[32];

            for (int i = 0; i < 32; i++)
                cipherKey[i] = (byte)Utilities.Random(0, byte.MaxValue, true);

            var AES = new AesCryptoServiceProvider()
            {
                BlockSize = 128,
                KeySize = 256,
                Key = cipherKey,
                //generate the symmetric algorithm key from the cipherkey
                IV = Encoding.UTF8.GetBytes(GetMD5Hash(Encoding.UTF8.GetString(cipherKey)).Substring(7, 16))
            };

            var encryptedStream = new MemoryStream();
            ICryptoTransform encryptor = AES.CreateEncryptor();

            using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
            {
                byte[] data = rawStream.ToArray();
                cryptoStream.Write(data, 0, data.Length);

                //get the data from the encrypted stream
                var encryptedData = encryptedStream.ToArray().ToList();
                //generate a random location within the data
                byte position = (byte)Utilities.Random(1, encryptedData.Count - 1);

                //insert the cipherkey at the random location, and the index to the key at the beginning of the data
                encryptedData.InsertRange(position, cipherKey);
                encryptedData.Insert(0, position);

                //encode the data as a base64 string
                string encryptedBase64 = Convert.ToBase64String(encryptedData.ToArray());

                //write the file in utf8
                File.WriteAllBytes(path, Encoding.UTF8.GetBytes(encryptedBase64));
            }
        }

        public static MemoryStream DecryptFile(string path)
        {
            //read the file data, convert it to a string, then decode from base64
            byte[] fileData = Convert.FromBase64String(Encoding.UTF8.GetString(File.ReadAllBytes(path)));
            //get the first byte as the position to the cipherkey
            byte pos = fileData[0];
            //read the cipher from the indexed location
            byte[] cipherKey = new byte[32];
            Buffer.BlockCopy(fileData, pos + 1, cipherKey, 0, 32);

            //generate the symmetric algorithm key from the cipherkey
            byte[] symmetric = Encoding.UTF8.GetBytes(GetMD5Hash(Encoding.UTF8.GetString(cipherKey)).Substring(7, 16));

            //remove the cipherkey and index from the data
            byte[] encryptedData = new byte[fileData.Length - 33];
            Buffer.BlockCopy(fileData, 1, encryptedData, 0, pos + 1);
            Buffer.BlockCopy(fileData, pos + 33, encryptedData, pos, fileData.Length - (pos + 33));

            var AES = new AesCryptoServiceProvider()
            {
                BlockSize = 128,
                KeySize = 256,
                Key = cipherKey,
                IV = symmetric
            };

            var encryptedStream = new MemoryStream(encryptedData);
            ICryptoTransform decryptor = AES.CreateDecryptor();

            //decrypt the data and return it as a stream
            var cryptpoStream = new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read);
            using (var reader = new StreamReader(cryptpoStream))
                return new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
        }
    }
}