using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Deepcoc
{
    

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }
    internal class MemoryReader
    {
        private readonly System.Diagnostics.Process _process;
        private readonly IntPtr _handle;

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr handle, IntPtr address, [Out()] byte[] buffer, int size, IntPtr bytesRead);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr handle, ulong address, [Out()] byte[] buffer, int size, out int bytesRead);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr handle, IntPtr address, byte[] buffer, int size, IntPtr numBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr handle, ulong address, byte[] buffer, int size, IntPtr numBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        public MemoryReader(System.Diagnostics.Process process)
        {
            _process = process;
            _handle = OpenProcess(ProcessAccessFlags.All, false, process.Id);
        }
        public IntPtr ReadAddress(IntPtr address, int[] offsets)
        {
            byte[] _result = new byte[16];
            IntPtr _nextAddress = IntPtr.Zero;
            ReadProcessMemory(_handle, address + offsets[0], _result, _result.Length, IntPtr.Zero);
            for(int i = 1; i < offsets.Length; i++)
            {
                _nextAddress = (IntPtr)BitConverter.ToInt64(_result,0) + offsets[i];
                ReadProcessMemory(_handle, (IntPtr)_nextAddress, _result, _result.Length, IntPtr.Zero);
            }
            return _nextAddress;
        }
        public IntPtr ReadAddress(IntPtr address, int offset)
        {
            byte[] _result = new byte[16];
            IntPtr _nextAddress = (IntPtr)(address + offset);
            ReadProcessMemory(_handle, (IntPtr)_nextAddress, _result, _result.Length, IntPtr.Zero);
            return _nextAddress;
        }

        public int ReadInt(IntPtr address, int offset)
        {
            byte[] _result = new byte[16];
            ReadProcessMemory(_handle, (IntPtr)(address + offset), _result, _result.Length, IntPtr.Zero);
            return BitConverter.ToInt32(_result,0);
        }
        public int ReadInt(IntPtr address)
        {
            byte[] _result = new byte[16];
            ReadProcessMemory(_handle, (IntPtr)(address), _result, _result.Length, IntPtr.Zero);
            return BitConverter.ToInt32(_result, 0);
        }
        public float ReadFloat(IntPtr address)
        {
            byte[] _result = new byte[16];
            ReadProcessMemory(_handle, (IntPtr)(address), _result, _result.Length, IntPtr.Zero);
            return BitConverter.ToSingle(_result, 0);  
        }
        public long ReadLong(IntPtr address)
        {
            byte[] _result = new byte[8];
            ReadProcessMemory(_handle, (IntPtr)(address), _result, _result.Length, IntPtr.Zero);
            return BitConverter.ToInt64(_result, 0);
        }
        public void WriteInt(IntPtr address, int value)
        {
            WriteProcessMemory(_handle, address, BitConverter.GetBytes(value), 4, IntPtr.Zero);
        }
        public void WriteLong(IntPtr address, long value)
        {
            WriteProcessMemory(_handle, address, BitConverter.GetBytes(value), 8, IntPtr.Zero);
        }
        public void WriteFloat(IntPtr address, float value)
        {
            WriteProcessMemory(_handle, address, BitConverter.GetBytes(value), 4, IntPtr.Zero);
            System.Diagnostics.Debug.WriteLine(value);
        }
    }
}
