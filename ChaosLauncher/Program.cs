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
