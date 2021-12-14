#region Using directives

using System;
using ProjectCeleste.Misc.Utils.Extension;

#endregion

namespace ProjectCeleste.Misc.Utils
{
    public sealed class OsVersionInfo
    {
        internal OsVersionInfo(string name, Version version)
        {
            name.ThrowIfNullOrWhiteSpace(nameof(name));
            version.ThrowIfNull(nameof(version));

            Name = name;
            Minor = version.Minor;
            Major = version.Major;
            Build = version.Build;
            FullName = "Microsoft " + Name + " [Version " + Major + "." + Minor + "." + Build + "]";
        }

        public string Name { get; }

        public string FullName { get; }

        public int Minor { get; }

        public int Major { get; }

        public int Build { get; }
    }

    public static class OsVersionUtils
    {
        private static OsVersionInfo _osVersionInfo;

        /// <summary>
        ///     Init OSVersionInfo object by current windows environment
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static OsVersionInfo GetOsVersionInfo()
        {
            if (_osVersionInfo != null)
                return _osVersionInfo;

            var osVersionObj = Environment.OSVersion;
            _osVersionInfo = new OsVersionInfo(GetOsName(osVersionObj), osVersionObj.Version);
            return _osVersionInfo;
        }

        /// <summary>
        ///     Get current windows name
        /// </summary>
        /// <param name="osInfo"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string GetOsName(OperatingSystem osInfo)
        {
            return osInfo.Platform switch
            {
                //for old windows kernel 
                PlatformID.Win32Windows => ForWin32Windows(osInfo),
                //fow NT kernel 
                PlatformID.Win32NT => ForWin32Nt(osInfo),
                PlatformID.MacOSX => throw new NotSupportedException(),
                PlatformID.Unix => throw new NotSupportedException(),
                PlatformID.Win32S => throw new NotSupportedException(),
                PlatformID.WinCE => throw new NotSupportedException(),
                PlatformID.Xbox => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(osInfo.Platform), osInfo.Platform, string.Empty)
            };
        }

        /// <summary>
        ///     for old windows kernel
        ///     this function is the child function for method GetOSName
        /// </summary>
        /// <param name="osInfo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string ForWin32Windows(OperatingSystem osInfo)
        {
            //Code to determine specific version of Windows 95,  
            //Windows 98, Windows 98 Second Edition, or Windows Me. 
            return osInfo.Version.Minor switch
            {
                0 => "Windows 95",
                10 => osInfo.Version.Revision.ToString() switch
                {
                    "2222A" => "Windows 98 Second Edition",
                    _ => "Windows 98"
                },
                90 => "Windows Me",
                _ => throw new ArgumentOutOfRangeException(nameof(osInfo.Version.Minor), osInfo.Version.Minor,
                    string.Empty)
            };
        }

        /// <summary>
        ///     fow NT kernel
        ///     this function is the child function for method GetOSName
        /// </summary>
        /// <param name="osInfo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string ForWin32Nt(OperatingSystem osInfo)
        {
            //Code to determine specific version of Windows NT 3.51,  
            //Windows NT 4.0, Windows 2000, or Windows XP. 
            return osInfo.Version.Major switch
            {
                3 => "Windows NT 3.51",
                4 => "Windows NT 4.0",
                5 => osInfo.Version.Minor switch
                {
                    0 => "Windows 2000",
                    1 => "Windows XP",
                    2 => "Windows 2003",
                    _ => throw new ArgumentOutOfRangeException(nameof(osInfo.Version.Minor), osInfo.Version.Minor,
                        string.Empty)
                },
                6 => osInfo.Version.Minor switch
                {
                    0 => "Windows Vista",
                    1 => "Windows 7",
                    2 => "Windows 8",
                    3 => "Windows 8.1",
                    _ => throw new ArgumentOutOfRangeException(nameof(osInfo.Version.Minor), osInfo.Version.Minor,
                        string.Empty)
                },
                10 => "Windows 10",
                _ => throw new ArgumentOutOfRangeException(nameof(osInfo.Version.Major), osInfo.Version.Major,
                    string.Empty)
            };
        }
    }
}