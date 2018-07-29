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
                Dns.GetHostEntry(Chaos.Paths.HostName).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
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
                Chaos.Paths.DarkAgesExe
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

                //if the option to inject dawnd.dll is checked
                if (injectDawndCbox.Checked)
                {
                    //get a handle for access
                    IntPtr accessHnd = SafeNativeMethods.OpenProcess(ProcessAccess.All, true, (uint)proc.Id);
                    //use access handle to inject dawnd.dll
                    InjectDLL(accessHnd,
#if DEBUG
                    $@"{Chaos.Paths.DarkAgesDir}dawnd.dll"
#else
                    "dawnd.dll"
#endif
                    );
                }

                //resume process
                memory.Position = 0x6F3CA4;
                SafeNativeMethods.ResumeThread(procInfo.ThreadHandle);
            }

            //let process render it's window before we change the title
            while (proc.MainWindowHandle == IntPtr.Zero) { }
            SafeNativeMethods.SetWindowText(proc.MainWindowHandle, "Chaos");
        }

        private void InjectDLL(IntPtr processHandle, string dllName)
        {
            //lla
            byte[] lib = new byte[] { 65, 121, 114, 97, 114, 98, 105, 76, 100, 97, 111, 76 };

            IntPtr outStuff;

            //length of string containing the DLL file name +1 byte padding
            int dllNameLength = dllName.Length + 1;
            //allocate memory within the virtual address space of the target process
            IntPtr processMemoryBase = SafeNativeMethods.VirtualAllocEx(processHandle, (IntPtr)null, (UIntPtr)dllNameLength, 0x1000, 0x40);
            //write DLL file name to allocated memory in target process
            SafeNativeMethods.WriteProcessMemory(processHandle, processMemoryBase, dllName, (UIntPtr)dllNameLength, out outStuff);
            //function pointer "Injector"
            UIntPtr moduleAddress = SafeNativeMethods.GetProcAddress(SafeNativeMethods.GetModuleHandle("kernel32.dll"), Encoding.ASCII.GetString(lib.Reverse().ToArray()));

            if (moduleAddress == null)
            {
                //invalid address
                MessageBox.Show(this, "Invalid module address.");
                return;
            }

            //create thread in target process, and store handle
            IntPtr threadHandle = SafeNativeMethods.CreateRemoteThread(processHandle, (IntPtr)null, UIntPtr.Zero, moduleAddress, processMemoryBase, 0, out outStuff);
            //make sure thread handle is valid
            if (threadHandle == null)
            {
                //invalid thread handle
                MessageBox.Show(this, "Invalid thread handle.");
                return;
            }
            //time-out is 5 seconds...
            WaitEventResult result = (WaitEventResult)SafeNativeMethods.WaitForSingleObject(threadHandle, 5000);
            //check whether thread timed out...
            if (result != WaitEventResult.Signaled)
            {
                //thread timed out...
                MessageBox.Show(this, "Thread timed out.");
                //make sure thread handle is valid before closing... prevents crashes.
                if (threadHandle != null)
                    SafeNativeMethods.CloseHandle(threadHandle);
                return;
            }
            //clear up allocated space ( Allocmem )
            SafeNativeMethods.VirtualFreeEx(processHandle, processMemoryBase, UIntPtr.Zero, 0x8000);
            //make sure thread handle is valid before closing... prevents crashes.
            if (threadHandle != null)
                SafeNativeMethods.CloseHandle(threadHandle);
            //return succeeded
            return;
        }
    }
}
