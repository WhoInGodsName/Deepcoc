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

        public IntPtr CreateCodeCave(int caveSize)
        {
            IntPtr caveAddress = VirtualAllocEx(_handle, IntPtr.Zero, caveSize, 0x1000 | 0x2000, 0x40);
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
            long jumpOperand = jumpAddress.ToInt64() - (instruction.ToInt64() + instructionLength + 5);
            byte[] jumpBytes = BitConverter.GetBytes((int)jumpOperand);

            byte[] result = new byte[5];
            result[0] = 0xE9; // JMP opcode
            Buffer.BlockCopy(jumpBytes, 0, result, 1, 4); // Copy the jump operand after the JMP opcode

            return result;
        }


        public void CreateJump(IntPtr jumpAddress, IntPtr instruction, int instructionLength)
        {
            byte[] bytes = CalculateJump(jumpAddress, instruction, instructionLength);
            WriteProcessMemory(_handle, instruction, bytes, bytes.Length, IntPtr.Zero);
        }

        public void CreateDetour(IntPtr instruction, int instructionLength, byte[] injectedInstructions = null, bool jumpBack = true)
        {
            byte[] originalInstructions = ReadBytes(instruction, instructionLength);

            // Create a code cave and write the original instructions to it
            IntPtr codeCaveAddress = CreateCodeCave(instructionLength);
            System.Diagnostics.Debug.WriteLine(codeCaveAddress.ToString("X"));

            WriteToCave(codeCaveAddress, originalInstructions);

            if (injectedInstructions != null)
            {
                // Write the injected instructions to the code cave
                WriteToCave(codeCaveAddress, injectedInstructions);

                if (jumpBack)
                {
                    // Calculate the jump offset to return to the original flow of execution
                    long jumpBackOffset = (codeCaveAddress.ToInt64() + injectedInstructions.Length) - (instruction.ToInt64() + instructionLength + 5);
                    byte[] jumpBackBytes = BitConverter.GetBytes((int)jumpBackOffset);

                    // Inject the jump offset directly into the original instruction
                    WriteProcessMemory(_handle, instruction, new byte[] { 0xE9 }, 1, IntPtr.Zero); // JMP opcode
                    WriteProcessMemory(_handle, instruction + 1, jumpBackBytes, 4, IntPtr.Zero);
                }
            }

            // Calculate the jump offset to the code cave
            long jumpOffset = codeCaveAddress.ToInt64() - (instruction.ToInt64() + instructionLength + 5);
            byte[] jumpBytes = BitConverter.GetBytes((int)jumpOffset);

            // Modify the original instruction to jump to the code cave
            WriteProcessMemory(_handle, instruction, new byte[] { 0xE9 }, 1, IntPtr.Zero); // JMP opcode
            WriteProcessMemory(_handle, instruction + 1, jumpBytes, 4, IntPtr.Zero);
        }


    }
}
