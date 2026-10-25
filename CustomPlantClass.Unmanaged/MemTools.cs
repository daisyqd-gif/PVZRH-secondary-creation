using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CustomPlantClass.Unmanaged
{
    /// <summary>
    /// Reads, writes, searches, and temporarily patches memory in the current process.
    /// </summary>
    public sealed class MemoryPatch : IDisposable
    {
        private const uint PageExecuteReadWrite = 0x40;

        private readonly object _syncRoot = new object();
        private readonly IntPtr _address;
        private readonly byte[] _replacement;
        private readonly byte[] _original;
        private bool _isEnabled;
        private bool _isDisposed;

        private MemoryPatch(IntPtr address, byte[] replacement, byte[] expected)
        {
            if (address == IntPtr.Zero)
            {
                throw new ArgumentException("补丁地址不能为 0。", nameof(address));
            }

            if (replacement == null || replacement.Length == 0)
            {
                throw new ArgumentException("补丁字节不能为空。", nameof(replacement));
            }

            _address = address;
            _replacement = (byte[])replacement.Clone();
            _original = ReadBytes(address, replacement.Length);

            if (expected != null && !BytesEqual(_original, expected))
            {
                throw new InvalidOperationException(
                    "目标位置的原始字节与预期不符。\n" +
                    "地址：0x" + address.ToInt64().ToString("X") + "\n" +
                    "预期：" + BytesToHex(expected) + "\n" +
                    "实际：" + BytesToHex(_original));
            }
        }

        /// <summary>
        /// Gets the address at which this patch is applied.
        /// </summary>
        public IntPtr Address
        {
            get { return _address; }
        }

        /// <summary>
        /// Gets whether the replacement bytes are currently applied.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        /// <summary>
        /// Creates a patch at a module-relative address.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module containing the patch.</param>
        /// <param name="rva">Relative virtual address of the patch.</param>
        /// <param name="replacement">Bytes to write when the patch is enabled.</param>
        /// <param name="expected">Optional bytes that must currently exist at the patch address.</param>
        /// <returns>A patch initialized with the bytes currently at the target address.</returns>
        public static MemoryPatch FromRva(string moduleName, long rva, byte[] replacement, byte[] expected = null)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException("模块名不能为空。", nameof(moduleName));
            }

            if (replacement == null || replacement.Length == 0)
            {
                throw new ArgumentException("补丁字节不能为空。", nameof(replacement));
            }

            ModuleInfo module = GetModuleInfo(moduleName);
            if (rva < 0 || rva > module.Size - (long)replacement.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rva),
                    "RVA 0x" + rva.ToString("X") + " 超出模块范围。");
            }

            return new MemoryPatch(Add(module.BaseAddress, rva), replacement, expected);
        }

        /// <summary>
        /// Creates a patch at an offset relative to a unique signature match.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module to search.</param>
        /// <param name="pattern">Space-separated hexadecimal bytes; use <c>?</c> or <c>??</c> as a wildcard.</param>
        /// <param name="patchOffset">Offset from the start of the matched signature.</param>
        /// <param name="replacement">Bytes to write when the patch is enabled.</param>
        /// <param name="expected">Optional bytes that must currently exist at the patch address.</param>
        /// <returns>A patch initialized with the bytes currently at the target address.</returns>
        public static MemoryPatch FromPattern(
            string moduleName,
            string pattern,
            int patchOffset,
            byte[] replacement,
            byte[] expected = null)
        {
            if (replacement == null || replacement.Length == 0)
            {
                throw new ArgumentException("补丁字节不能为空。", nameof(replacement));
            }

            IntPtr matchAddress = FindUniquePattern(moduleName, pattern);
            return new MemoryPatch(Add(matchAddress, patchOffset), replacement, expected);
        }

        /// <summary>
        /// Applies the replacement bytes, unless the patch is already enabled.
        /// </summary>
        public void Enable()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                if (_isEnabled)
                {
                    return;
                }

                byte[] current = ReadBytes(_address, _original.Length);
                if (!BytesEqual(current, _original))
                {
                    throw new InvalidOperationException(
                        "启用补丁失败：目标内存已经被其他代码修改。\n" +
                        "地址：0x" + _address.ToInt64().ToString("X") + "\n" +
                        "预期：" + BytesToHex(_original) + "\n" +
                        "实际：" + BytesToHex(current));
                }

                WriteBytes(_address, _replacement);
                _isEnabled = true;
            }
        }

        /// <summary>
        /// Restores the original bytes, unless the patch is already disabled.
        /// </summary>
        public void Disable()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                if (!_isEnabled)
                {
                    return;
                }

                byte[] current = ReadBytes(_address, _replacement.Length);
                if (!BytesEqual(current, _replacement))
                {
                    throw new InvalidOperationException(
                        "恢复补丁失败：补丁位置已经被其他代码修改。\n" +
                        "地址：0x" + _address.ToInt64().ToString("X") + "\n" +
                        "预期：" + BytesToHex(_replacement) + "\n" +
                        "实际：" + BytesToHex(current));
                }

                WriteBytes(_address, _original);
                _isEnabled = false;
            }
        }

        /// <summary>
        /// Restores the original bytes if needed and releases this patch object.
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (_isEnabled)
                {
                    Disable();
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Finds the only occurrence of a byte pattern in a loaded module.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module to search.</param>
        /// <param name="pattern">Space-separated hexadecimal bytes; use <c>?</c> or <c>??</c> as a wildcard.</param>
        /// <returns>The address of the pattern's first byte.</returns>
        /// <exception cref="InvalidOperationException">The pattern is not found or occurs more than once.</exception>
        public static IntPtr FindUniquePattern(string moduleName, string pattern)
        {
            ModuleInfo module = GetModuleInfo(moduleName);
            PatternByte[] bytes = ParsePattern(pattern);
            if (bytes.Length > module.Size)
            {
                throw new InvalidOperationException("特征码长度超过模块大小。");
            }

            byte[] memory = ReadBytes(module.BaseAddress, module.Size);
            int match = -1;
            int matchCount = 0;
            int lastStart = memory.Length - bytes.Length;

            for (int start = 0; start <= lastStart; start++)
            {
                if (!MatchesAt(memory, bytes, start))
                {
                    continue;
                }

                match = start;
                matchCount++;
                if (matchCount > 1)
                {
                    break;
                }
            }

            if (matchCount == 0)
            {
                throw new InvalidOperationException("在 " + moduleName + " 中找不到特征码：\n" + pattern);
            }

            if (matchCount > 1)
            {
                throw new InvalidOperationException("特征码在 " + moduleName + " 中不是唯一的：\n" + pattern);
            }

            return Add(module.BaseAddress, match);
        }

        /// <summary>
        /// Finds the first occurrence of a byte pattern in a loaded module.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module to search.</param>
        /// <param name="pattern">Space-separated hexadecimal bytes; use <c>?</c> or <c>??</c> as a wildcard.</param>
        /// <returns>The address of the first match.</returns>
        public static IntPtr FindFirstPattern(string moduleName, string pattern)
        {
            ModuleInfo module = GetModuleInfo(moduleName);
            PatternByte[] bytes = ParsePattern(pattern);
            if (bytes.Length > module.Size)
            {
                throw new InvalidOperationException("特征码长度超过模块大小。");
            }

            byte[] memory = ReadBytes(module.BaseAddress, module.Size);
            int lastStart = memory.Length - bytes.Length;
            for (int start = 0; start <= lastStart; start++)
            {
                if (MatchesAt(memory, bytes, start))
                {
                    return Add(module.BaseAddress, start);
                }
            }

            throw new InvalidOperationException("在 " + moduleName + " 中找不到特征码：\n" + pattern);
        }

        /// <summary>
        /// Resolves the target address of an instruction containing a 32-bit RIP-relative displacement.
        /// </summary>
        /// <param name="instructionAddress">Address of the instruction's first byte.</param>
        /// <param name="displacementOffset">Offset of the displacement within the instruction.</param>
        /// <param name="instructionLength">Total length of the instruction in bytes.</param>
        /// <returns>The address computed from the instruction end and its relative displacement.</returns>
        public static IntPtr ResolveRipRelativeAddress(
            IntPtr instructionAddress,
            int displacementOffset,
            int instructionLength)
        {
            if (instructionAddress == IntPtr.Zero)
            {
                throw new ArgumentException("指令地址不能为 0。", nameof(instructionAddress));
            }

            int displacement = Marshal.ReadInt32(Add(instructionAddress, displacementOffset));
            return Add(instructionAddress, instructionLength + (long)displacement);
        }

        /// <summary>
        /// Reads a single-precision floating-point value from memory.
        /// </summary>
        /// <param name="address">Address from which to read.</param>
        /// <returns>The floating-point value stored at the address.</returns>
        public static float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(ReadBytes(address, sizeof(float)), 0);
        }

        /// <summary>
        /// Writes a single-precision floating-point value to memory.
        /// </summary>
        /// <param name="address">Address to write to.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteFloat(IntPtr address, float value)
        {
            WriteBytes(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Reads a sequence of bytes from memory.
        /// </summary>
        /// <param name="address">Address from which to read.</param>
        /// <param name="length">Number of bytes to read; must be greater than zero.</param>
        /// <returns>The bytes read from memory.</returns>
        public static byte[] ReadBytes(IntPtr address, int length)
        {
            if (address == IntPtr.Zero)
            {
                throw new ArgumentException("读取地址不能为 0。", nameof(address));
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            byte[] bytes = new byte[length];
            Marshal.Copy(address, bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// Writes a sequence of bytes to memory and flushes the instruction cache.
        /// </summary>
        /// <param name="address">Address to write to.</param>
        /// <param name="bytes">Bytes to write; the array must not be empty.</param>
        public static void WriteBytes(IntPtr address, byte[] bytes)
        {
            if (address == IntPtr.Zero)
            {
                throw new ArgumentException("写入地址不能为 0。", nameof(address));
            }

            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("写入字节不能为空。", nameof(bytes));
            }

            UIntPtr size = new UIntPtr((uint)bytes.Length);
            uint oldProtection;
            if (!VirtualProtect(address, size, PageExecuteReadWrite, out oldProtection))
            {
                throw new InvalidOperationException(
                    "VirtualProtect 失败，错误码：" +
                    Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
            }

            Exception writeError = null;
            try
            {
                Marshal.Copy(bytes, 0, address, bytes.Length);
                if (!FlushInstructionCache(GetCurrentProcess(), address, size))
                {
                    writeError = new InvalidOperationException(
                        "FlushInstructionCache 失败，错误码：" +
                        Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
                }
            }
            finally
            {
                uint ignoredProtection;
                if (!VirtualProtect(address, size, oldProtection, out ignoredProtection) && writeError == null)
                {
                    writeError = new InvalidOperationException(
                        "恢复内存保护失败，错误码：" +
                        Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
                }
            }

            if (writeError != null)
            {
                throw writeError;
            }
        }

        /// <summary>
        /// Converts hexadecimal text to bytes. Spaces, commas, tabs, and line breaks separate bytes.
        /// </summary>
        /// <param name="hex">Hexadecimal byte values to parse.</param>
        /// <returns>The parsed bytes, or an empty array for null, empty, or whitespace input.</returns>
        public static byte[] HexToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return new byte[0];
            }

            string[] tokens = hex.Split(
                new[] { ' ', ',', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                bytes[i] = byte.Parse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return bytes;
        }

        /// <summary>
        /// Converts bytes to uppercase, space-separated hexadecimal text.
        /// </summary>
        /// <param name="bytes">Bytes to format.</param>
        /// <returns>The formatted bytes, or an empty string when the array is null.</returns>
        public static string BytesToHex(byte[] bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            string[] tokens = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                tokens[i] = bytes[i].ToString("X2", CultureInfo.InvariantCulture);
            }

            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Gets the base address of a loaded module in the current process.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module.</param>
        /// <returns>The module's base address.</returns>
        public static IntPtr GetModuleBaseAddress(string moduleName)
        {
            return GetModuleInfo(moduleName).BaseAddress;
        }

        /// <summary>
        /// Converts an absolute address to an RVA relative to a loaded module.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module.</param>
        /// <param name="address">Absolute address to convert.</param>
        /// <returns>The offset of the address from the module's base address.</returns>
        public static long AddressToRva(string moduleName, IntPtr address)
        {
            ModuleInfo module = GetModuleInfo(moduleName);
            return address.ToInt64() - module.BaseAddress.ToInt64();
        }

        /// <summary>
        /// Converts a module-relative address to an absolute address.
        /// </summary>
        /// <param name="moduleName">Name of the loaded module.</param>
        /// <param name="rva">Relative virtual address within the module.</param>
        /// <returns>The corresponding absolute address.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The RVA is outside the module.</exception>
        public static IntPtr RvaToAddress(string moduleName, long rva)
        {
            ModuleInfo module = GetModuleInfo(moduleName);
            if (rva < 0 || rva >= module.Size)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rva),
                    "RVA 0x" + rva.ToString("X") + " 超出模块范围。");
            }

            return Add(module.BaseAddress, rva);
        }

        private static ModuleInfo GetModuleInfo(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException("模块名不能为空。", nameof(moduleName));
            }

            using (Process process = Process.GetCurrentProcess())
            {
                foreach (ProcessModule module in process.Modules)
                {
                    if (string.Equals(module.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new ModuleInfo(module.BaseAddress, module.ModuleMemorySize);
                    }
                }
            }

            throw new DllNotFoundException("当前进程中找不到模块：" + moduleName);
        }

        private static PatternByte[] ParsePattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("特征码不能为空。", nameof(pattern));
            }

            string[] tokens = pattern.Split(
                new[] { ' ', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
            PatternByte[] bytes = new PatternByte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "?" || tokens[i] == "??")
                {
                    bytes[i] = new PatternByte(0, true);
                }
                else
                {
                    bytes[i] = new PatternByte(
                        byte.Parse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                        false);
                }
            }

            return bytes;
        }

        private static bool MatchesAt(byte[] memory, PatternByte[] pattern, int start)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (!pattern[i].IsWildcard && memory[start + i] != pattern[i].Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool BytesEqual(byte[] first, byte[] second)
        {
            if (first == null || second == null || first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static IntPtr Add(IntPtr address, long offset)
        {
            return new IntPtr(address.ToInt64() + offset);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(MemoryPatch));
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(
            IntPtr address,
            UIntPtr size,
            uint newProtection,
            out uint oldProtection);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);

        private struct ModuleInfo
        {
            public ModuleInfo(IntPtr baseAddress, int size)
            {
                BaseAddress = baseAddress;
                Size = size;
            }

            public IntPtr BaseAddress { get; private set; }

            public int Size { get; private set; }
        }

        private struct PatternByte
        {
            public PatternByte(byte value, bool isWildcard)
            {
                Value = value;
                IsWildcard = isWildcard;
            }

            public byte Value { get; private set; }

            public bool IsWildcard { get; private set; }
        }
    }
}
