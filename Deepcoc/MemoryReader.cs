using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Deepcoc
{
    
    internal class MemoryReader
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr handle, IntPtr address, [Out()] byte[] buffer, int size, out int bytesRead);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr handle, ulong address, [Out()] byte[] buffer, int size, out int bytesRead);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
    }
}
