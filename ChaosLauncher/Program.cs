using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ChaosLauncher
{
    internal class Program
    {
        [STAThread]
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
