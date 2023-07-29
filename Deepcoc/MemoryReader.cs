using System.Runtime.InteropServices;

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
        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr handle, IntPtr address, int size, uint allocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualFreeEx(IntPtr handle, UIntPtr address, int size, int freeType);
        [DllImport("kernel32.dll")]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        public MemoryReader(System.Diagnostics.Process process)
        {
            _process = process;
            _handle = OpenProcess(ProcessAccessFlags.All, false, process.Id);
        }

        public bool SetMemoryProtection(IntPtr address, int size, uint protectionFlags)
        {
            return VirtualProtectEx(_handle, address, size, protectionFlags, out _);
        }

        public IntPtr ReadAddress(IntPtr address, int[] offsets)
        {
            byte[] _result = new byte[16];
            IntPtr _nextAddress = IntPtr.Zero;
            ReadProcessMemory(_handle, address + offsets[0], _result, _result.Length, IntPtr.Zero);
            for (int i = 1; i < offsets.Length; i++)
            {
                _nextAddress = (IntPtr)BitConverter.ToInt64(_result, 0) + offsets[i];
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
            return BitConverter.ToInt32(_result, 0);
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
        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] _result = new byte[size];
            ReadProcessMemory(_handle, address, _result, _result.Length, IntPtr.Zero);
            return _result;
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

        public IntPtr CreateCodeCave(IntPtr lpAddress, int caveSize)
        {
            IntPtr caveAddress = VirtualAllocEx(_handle, IntPtr.Zero, caveSize, 0x1000 | 0x2000 | 0x00100000, 0x40);
            return caveAddress;
        }
        public IntPtr WriteToCave(IntPtr caveAddress, byte[] code)
        {
            WriteProcessMemory(_handle, caveAddress, code, code.Length, IntPtr.Zero);
            return caveAddress;
        }

        public void FreeCave(UIntPtr caveAddress)
        {
            var rel = VirtualFreeEx(_handle, caveAddress, 0, 0x00008000);
        }

        public byte[] CalculateJump(IntPtr jumpAddress, IntPtr instruction, int instructionLength)
        {
            long jumpOperand = jumpAddress.ToInt64() - (instruction.ToInt64() + instructionLength + 14);
            byte[] jumpBytes = BitConverter.GetBytes(jumpOperand);

            byte[] result = new byte[14];
            result[0] = 0xFF; // JMP opcode (FF25 in x86_64)
            result[1] = 0x25; // ModR/M byte for indirect jump in x86_64
            Buffer.BlockCopy(jumpBytes, 0, result, 2, 8); // Copy the jump operand after the ModR/M byte

            return result;
        }


        public void CreateJump(IntPtr jumpAddress, IntPtr instruction, int instructionLength)
        {
            byte[] bytes = CalculateJump(jumpAddress, instruction, instructionLength);
            WriteProcessMemory(_handle, instruction, bytes, bytes.Length, IntPtr.Zero);
        }

        public IntPtr CreateDetour(IntPtr pSource, IntPtr pDestination, int dwLen)
        {
            int minLen = 14;

            if (dwLen < minLen)
                return IntPtr.Zero;

            // Check if the target address is writable
            uint oldProtect = 0;
            if (!SetMemoryProtection(pSource, dwLen, 0x40))
            {
                // Failed to set the memory protection, handle the error appropriately
                return IntPtr.Zero;
            }

            byte[] stub = new byte[]
            {
                0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, // jmp qword ptr [$+6]
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 // ptr
            };

            // Allocate memory for the detour
            IntPtr pTrampoline = VirtualAllocEx(_handle, IntPtr.Zero, dwLen + stub.Length, 0x1000 | 0x2000, 0x40);

            if (pTrampoline == IntPtr.Zero)
                return IntPtr.Zero;

            byte[] originalBytes = new byte[dwLen];
            ReadProcessMemory(_handle, pSource, originalBytes, dwLen, IntPtr.Zero);

            // Trampoline
            long retto = pSource.ToInt64() + dwLen;
            Buffer.BlockCopy(BitConverter.GetBytes(retto), 0, stub, 6, 8);
            WriteProcessMemory(_handle, pTrampoline, originalBytes, dwLen, IntPtr.Zero);
            WriteProcessMemory(_handle, pTrampoline + dwLen, stub, stub.Length, IntPtr.Zero);

            // Original
            long destinationAddr = pDestination.ToInt64();
            Buffer.BlockCopy(BitConverter.GetBytes(destinationAddr), 0, stub, 6, 8);
            WriteProcessMemory(_handle, pSource, stub, stub.Length, IntPtr.Zero);

            // Fill with NOPs
            byte[] nops = new byte[dwLen - minLen];
            for (int i = 0; i < nops.Length; i++)
            {
                nops[i] = 0x90; // NOP opcode
            }
            WriteProcessMemory(_handle, pSource + minLen, nops, nops.Length, IntPtr.Zero);

            // Restore memory protection
            VirtualProtect(pSource, dwLen, oldProtect, out _);

            return pTrampoline;
        }




    }
}
