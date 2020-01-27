using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class SymbolicLinkUtils
    {
        public enum SymbolicLinkFlag
        {
            File = 0,
            Directory = 1
        }

        private const int CREATION_DISPOSITION_OPEN_EXISTING = 3;
        private const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName,
            SymbolicLinkFlag dwFlags);

        [UsedImplicitly]
        public static bool CreateSymLink([NotNull] string lpSymlinkFileName, [NotNull] string lpTargetFileName)
        {
            lpSymlinkFileName.ThrowIfNullOrWhiteSpace(nameof(lpSymlinkFileName));
            lpTargetFileName.ThrowIfNullOrWhiteSpace(nameof(lpTargetFileName));

            return CreateSymbolicLink(lpSymlinkFileName, lpTargetFileName,
                DirectoryUtils.IsDirectory(lpTargetFileName) ? SymbolicLinkFlag.Directory : SymbolicLinkFlag.File);
        }

        [UsedImplicitly]
        public static bool CreateSymLink([NotNull] string lpSymlinkFileName, [NotNull] string lpTargetFileName,
            SymbolicLinkFlag dwFlags)
        {
            lpSymlinkFileName.ThrowIfNullOrWhiteSpace(nameof(lpSymlinkFileName));
            lpTargetFileName.ThrowIfNullOrWhiteSpace(nameof(lpTargetFileName));

            return CreateSymbolicLink(lpSymlinkFileName, lpTargetFileName, dwFlags);
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsSymLinkPath([NotNull] string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            return IsSymLinkPath(path,
                DirectoryUtils.IsDirectory(path) ? SymbolicLinkFlag.Directory : SymbolicLinkFlag.File);
        }

        [UsedImplicitly]
        [Pure]
        public static bool IsSymLinkPath([NotNull] string path, SymbolicLinkFlag dwFlags)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            return dwFlags switch
            {
                SymbolicLinkFlag.File => ((new FileInfo(path).Attributes & FileAttributes.ReparsePoint) != 0),
                SymbolicLinkFlag.Directory => ((new DirectoryInfo(path).Attributes & FileAttributes.ReparsePoint) != 0),
                _ => throw new ArgumentOutOfRangeException(nameof(dwFlags), dwFlags, null)
            };
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
            IntPtr securityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode,
            SetLastError = true)]
        private static extern int GetFinalPathNameByHandle([In] IntPtr hFile, [Out] StringBuilder lpszFilePath,
            [In] int cchFilePath, [In] int dwFlags);

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string GetSymLinkRealPath([NotNull] string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            if (!IsSymLinkPath(path))
                throw new Exception("Path is not an symlink");

            if (!Directory.Exists(path) && !File.Exists(path))
                throw new FileNotFoundException("Path not found", path);

            var symlink = new DirectoryInfo(path); // No matter if it's a file or folder
            var directoryHandle = CreateFile(symlink.FullName, 0, 2, IntPtr.Zero, CREATION_DISPOSITION_OPEN_EXISTING,
                FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero); //Handle file / folder

            if (directoryHandle.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var result = new StringBuilder(512);
            var mResult = GetFinalPathNameByHandle(directoryHandle.DangerousGetHandle(), result, result.Capacity, 0);

            if (mResult < 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (result.Length >= 4 && result[0] == '\\' && result[1] == '\\' && result[2] == '?' && result[3] == '\\')
                return result.ToString().Substring(4); // "\\?\" remove

            return result.ToString();
        }
    }
}