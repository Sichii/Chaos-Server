﻿namespace ChaosLauncher
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class ProcMemoryStream : Stream, IDisposable
    {
        private ProcessAccess AccessType;
        private bool Disposed;
        private IntPtr ProcessHandle;
        public override long Position { get; set; }
        public int ProcessId { get; set; }
        public override bool CanRead => (AccessType & ProcessAccess.VmRead) > ProcessAccess.None;
        public override bool CanSeek => true;
        public override bool CanWrite => (AccessType & (ProcessAccess.VmOperation | ProcessAccess.VmWrite)) > ProcessAccess.None;

        public override long Length
        {
            get { throw new NotSupportedException("Length unsupported."); }
        }

        public override void Flush()
        {
            throw new NotSupportedException("Flush unsupported.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Length unsupported.");
        }

        internal ProcMemoryStream(ProcInfo procInfo, ProcessAccess access)
        {
            AccessType = access;
            ProcessId = procInfo.ProcessId;
            ProcessHandle = procInfo.ProcessHandle;
            
            if (ProcessHandle == IntPtr.Zero)
                throw new ArgumentException("Unable to open the process.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException("ProcMemoryStream");
            if (ProcessHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process is not open.");
            IntPtr num = Marshal.AllocHGlobal(count);
            if (num == IntPtr.Zero)
                throw new InvalidOperationException("Unable to allocate memory.");
            int bytesRead = 0;
            Kernel32.ReadProcessMemory(ProcessHandle, (IntPtr)Position, num, count, out bytesRead);
            Position += bytesRead;
            Marshal.Copy(num, buffer, offset, count);
            Marshal.FreeHGlobal(num);
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (Disposed)
                throw new ObjectDisposedException("ProcMemoryStream");
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    throw new NotSupportedException("SeekOrigin.End unsupported.");
            }
            return Position;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException("ProcMemoryStream");
            if (ProcessHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process is not open.");
            IntPtr allocDestination = Marshal.AllocHGlobal(count);
            if (allocDestination == IntPtr.Zero)
                throw new InvalidOperationException("Unable to allocate memory.");
            Marshal.Copy(buffer, offset, allocDestination, count);
            int bytes = 0;
            Kernel32.WriteProcessMemory(ProcessHandle, (IntPtr)Position, allocDestination, count, out bytes);
            Position += bytes;
            Marshal.FreeHGlobal(allocDestination);
        }

        public override void WriteByte(byte value) => Write(new byte[1] { value }, 0, 1);

        public void WriteString(string value) => Write(Encoding.ASCII.GetBytes(value), 0, value.Length);

        public override void Close()
        {
            if (Disposed)
                throw new ObjectDisposedException("ProcMemoryStream");
            if (ProcessHandle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero;
            }
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (ProcessHandle != IntPtr.Zero)
                {
                    Kernel32.CloseHandle(ProcessHandle);
                    ProcessHandle = IntPtr.Zero;
                }
                base.Dispose(disposing);
            }
            Disposed = true;
        }

        ~ProcMemoryStream()
        {
            Dispose(false);
        }
    }
}
