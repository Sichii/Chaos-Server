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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ChaosLauncher
{
    internal partial class Launcher : Form
    {
        internal Launcher()
        {
            InitializeComponent();
        }

        private void launchBtn_Click(object sender, EventArgs e)
        {
            StartInfo startInfo = new StartInfo();
            ProcInfo procInfo = new ProcInfo();
            startInfo.Size = Marshal.SizeOf(startInfo);

            //grab the server ip from the server dns, and your own ip
            IPAddress serverIP = 
#if DEBUG
                IPAddress.Loopback;
#else
                Dns.GetHostEntry(Paths.HostName).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
#endif

            IPAddress clientIP = null;

            WebRequest request = WebRequest.Create("https://checkip.amazonaws.com");
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                clientIP = IPAddress.Parse(reader.ReadToEnd().Trim('\n'));
            }


                //create the process
                SafeNativeMethods.CreateProcess(
#if DEBUG
                Paths.DarkAgesExe
#else
                @"Darkages.exe"
#endif
                , null, IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.Suspended, IntPtr.Zero, null, ref startInfo, out procInfo);

            //grab the process we created
            Process proc = Process.GetProcessById(procInfo.ProcessId);

            using (ProcMemoryStream memory = new ProcMemoryStream(procInfo, ProcessAccess.VmOperation | ProcessAccess.VmRead | ProcessAccess.VmWrite))
            {
                //force "socket" - call for direct ip
                memory.Position = 0x4333A2;
                memory.WriteByte(0xEB);

                //edit the direct ip to the server ip
                byte[] address = serverIP.Equals(clientIP) ? IPAddress.Loopback.GetAddressBytes() : serverIP.GetAddressBytes();
                memory.Position = 0x4333C2;
                memory.WriteByte(106);
                memory.WriteByte(address[3]);
                memory.WriteByte(106);
                memory.WriteByte(address[2]);
                memory.WriteByte(106);
                memory.WriteByte(address[1]);
                memory.WriteByte(106);
                memory.WriteByte(address[0]);

                //skip intro
                memory.Position = 0x42E61F;
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);

                //allow multiple instances
                memory.Position = 0x57A7D9;
                memory.WriteByte(0xEB);

                //resume process
                memory.Position = 0x6F3CA4;
                SafeNativeMethods.ResumeThread(procInfo.ThreadHandle);
            }

            //let process render it's window before we change the title
            while (proc.MainWindowHandle == IntPtr.Zero) { }
            SafeNativeMethods.SetWindowText(proc.MainWindowHandle, "Chaos");
        }
    }
}
