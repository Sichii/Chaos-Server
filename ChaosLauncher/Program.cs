using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChaosLauncher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("dawnd.dll"))
                using (MemoryStream data = new MemoryStream())
                using (Stream dawnd = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChaosLauncher.dawnd.dll"))
                {
                    dawnd.CopyTo(data);
                    File.WriteAllBytes("dawnd.dll", data.ToArray());
                }

            Application.Run(new Launcher());
        }
    }
}
