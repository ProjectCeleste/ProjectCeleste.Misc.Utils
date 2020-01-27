using System;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class SecureStringExtensions
    {
        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string GetValue([NotNull] this SecureString secureString)
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