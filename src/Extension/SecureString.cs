using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class SecureStringExtensions
    {
        public static string GetValue(this SecureString secureString)
        {
            secureString.ThrowIfNull(nameof(secureString));

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString) ?? throw new InvalidOperationException();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}