using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace SevenZipWrapper
{
    internal class SevenZipFormat : IDisposable
    {
        #region Win32 API

        private const string Kernel32Dll = "kernel32.dll";

        private sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeLibraryHandle() : base(true)
            {
            }

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Kernel32Dll)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool FreeLibrary(IntPtr hModule);

            /// <summary>Release library handle</summary>
            /// <returns>true if the handle was released</returns>
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            protected override bool ReleaseHandle()
            {
                return FreeLibrary(this.handle);
            }
        }

        [DllImport(Kernel32Dll, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibrary(
            [MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

        [DllImport(Kernel32Dll, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(
            SafeLibraryHandle hModule,
            [MarshalAs(UnmanagedType.LPStr)] string procName);

        #endregion

        private SafeLibraryHandle LibHandle;

        public SevenZipFormat(string sevenZipLibPath)
        {
            this.LibHandle = LoadLibrary(sevenZipLibPath);
            if (this.LibHandle.IsInvalid)
            {
                throw new Win32Exception();
            }

            IntPtr FunctionPtr = GetProcAddress(this.LibHandle, "GetHandlerProperty");
            // Not valid dll
            if (FunctionPtr == IntPtr.Zero)
            {
                this.LibHandle.Close();
                throw new ArgumentException();
            }
        }

        ~SevenZipFormat()
        {
            this.Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if ((this.LibHandle != null) && !this.LibHandle.IsClosed)
            {
                this.LibHandle.Close();
            }
            this.LibHandle = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IInArchive CreateInArchive(Guid classId)
        {
            if (this.LibHandle == null)
            {
                throw new ObjectDisposedException("SevenZipFormat");
            }

            CreateObjectDelegate CreateObject =
                (CreateObjectDelegate) Marshal.GetDelegateForFunctionPointer(
                    GetProcAddress(this.LibHandle, "CreateObject"), typeof (CreateObjectDelegate));

            if (CreateObject != null)
            {
                object Result;
                Guid InterfaceId = typeof (IInArchive).GUID;
                CreateObject(ref classId, ref InterfaceId, out Result);
                return Result as IInArchive;
            }

            return null;
        }

        private static Dictionary<KnownSevenZipFormat, Guid> FFormatClassMap;

        private static Dictionary<KnownSevenZipFormat, Guid> FormatClassMap
        {
            get
            {
                if (FFormatClassMap == null)
                {
                    FFormatClassMap = new Dictionary<KnownSevenZipFormat, Guid>();
                    FFormatClassMap.Add(KnownSevenZipFormat.SevenZip, new Guid("23170f69-40c1-278a-1000-000110070000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Arj, new Guid("23170f69-40c1-278a-1000-000110040000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.BZip2, new Guid("23170f69-40c1-278a-1000-000110020000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Cab, new Guid("23170f69-40c1-278a-1000-000110080000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Chm, new Guid("23170f69-40c1-278a-1000-000110e90000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Compound, new Guid("23170f69-40c1-278a-1000-000110e50000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Cpio, new Guid("23170f69-40c1-278a-1000-000110ed0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Deb, new Guid("23170f69-40c1-278a-1000-000110ec0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.GZip, new Guid("23170f69-40c1-278a-1000-000110ef0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Iso, new Guid("23170f69-40c1-278a-1000-000110e70000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Lzh, new Guid("23170f69-40c1-278a-1000-000110060000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Lzma, new Guid("23170f69-40c1-278a-1000-0001100a0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Nsis, new Guid("23170f69-40c1-278a-1000-000110090000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Rar, new Guid("23170f69-40c1-278a-1000-000110030000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Rpm, new Guid("23170f69-40c1-278a-1000-000110eb0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Split, new Guid("23170f69-40c1-278a-1000-000110ea0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Tar, new Guid("23170f69-40c1-278a-1000-000110ee0000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Wim, new Guid("23170f69-40c1-278a-1000-000110e60000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Z, new Guid("23170f69-40c1-278a-1000-000110050000"));
                    FFormatClassMap.Add(KnownSevenZipFormat.Zip, new Guid("23170f69-40c1-278a-1000-000110010000"));
                }
                return FFormatClassMap;
            }
        }

        public static Guid GetClassIdFromKnownFormat(KnownSevenZipFormat format)
        {
            Guid Result;
            if (FormatClassMap.TryGetValue(format, out Result))
            {
                return Result;
            }
            return Guid.Empty;
        }
    }
}