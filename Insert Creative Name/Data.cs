using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal static class Data
    {
        private static BinaryFormatter bf = new BinaryFormatter();

        internal static void Serialize(string path, object obj)
        {
            using (FileStream file = File.Create(path))
                bf.Serialize(file, obj);
        }

        internal static object Deserialize(string path)
        {
            using (FileStream file = File.OpenRead(path))
                return bf.Deserialize(file);
        }
    }
}
