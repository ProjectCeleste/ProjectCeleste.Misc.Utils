using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    /// <summary>
    ///     Generates a 16 byte Unique Identification code of a computer
    ///     Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
    ///     https://www.codeproject.com/Articles/28678/Generating-Unique-Key-Finger-Print-for-a-Computer
    /// </summary>
    public static class FingerPrintProvider
    {
        private static string _fingerPrint;

        public static string GetValue()
        {
            if (_fingerPrint != null)
                return _fingerPrint;

            try
            {
                var listId = new[]
                {
                    CpuId(),
                    BiosId(),
                    BaseId(),
                    VideoId(),
                    MacId()
                    //diskId()
                };

                if (listId.All(string.IsNullOrWhiteSpace))
                    throw new Exception();

                _fingerPrint = GetHash("CPU >> " + listId[0] +
                                       "\nBIOS >> " + listId[1] +
                                       "\nBASE >> " + listId[2] +
                                       "\nVIDEO >> " + listId[3] +
                                       "\nMAC >> " + listId[4]
                    //"\nDISK >> " + listId[5]
                );
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch //AV Block?
            {
                _fingerPrint = string.Empty;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return _fingerPrint;
        }

        public static async Task<string> GetValueAsync(CancellationToken ct = default)
        {
            return _fingerPrint ?? await Task.Factory.StartNew(GetValue, ct);
        }

        private static string GetHash(string s)
        {
            s.ThrowIfNull(nameof(s));

            MD5 sec = new MD5CryptoServiceProvider();
            var enc = new ASCIIEncoding();
            var bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(IReadOnlyList<byte> bt)
        {
            bt.ThrowIfNull(nameof(bt));

            var s = string.Empty;
            for (var i = 0; i < bt.Count; i++)
            {
                int n = bt[i];
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char) (n2 - 10 + 'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char) (n1 - 10 + 'A')).ToString();
                else
                    s += n1.ToString();
                if (i + 1 != bt.Count && (i + 1) % 2 == 0) s += "-";
            }

            return s;
        }

        #region Original Device ID Getting Code

        //Return a hardware identifier
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            var result = string.Empty;
            var mc = new ManagementClass(wmiClass);
            foreach (var o in mc.GetInstances())
            {
                var mo = (ManagementObject) o;
                if (mo[wmiMustBeTrue].ToString() != "True")
                    continue;
                if (!string.IsNullOrWhiteSpace(result))
                    continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    // ignored
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return result;
        }

        //Return a hardware identifier
        private static string Identifier(string wmiClass, string wmiProperty)
        {
            var result = string.Empty;
            var mc = new ManagementClass(wmiClass);
            foreach (var o in mc.GetInstances())
                //Only get the first one
            {
                var mo = (ManagementObject) o;
                if (!string.IsNullOrWhiteSpace(result))
                    continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    // ignored
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return result;
        }

        private static string CpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            var retVal = Identifier("Win32_Processor", "UniqueId");

            if (!string.IsNullOrWhiteSpace(retVal))
                return retVal;

            retVal = Identifier("Win32_Processor", "ProcessorId");

            if (!string.IsNullOrWhiteSpace(retVal))
                return retVal;

            retVal = Identifier("Win32_Processor", "Name");
            if (string.IsNullOrWhiteSpace(retVal)) //If no Name, use Manufacturer
                retVal = Identifier("Win32_Processor", "Manufacturer");
            //Add clock speed for extra security
            retVal += Identifier("Win32_Processor", "MaxClockSpeed");
            return retVal;
        }

        //BIOS Identifier
        private static string BiosId()
        {
            return Identifier("Win32_BIOS", "Manufacturer")
                   + Identifier("Win32_BIOS", "SMBIOSBIOSVersion")
                   + Identifier("Win32_BIOS", "IdentificationCode")
                   + Identifier("Win32_BIOS", "SerialNumber")
                   + Identifier("Win32_BIOS", "ReleaseDate")
                   + Identifier("Win32_BIOS", "Version");
        }

        //Main physical hard drive ID
        //private static string DiskId()
        //{
        //    return identifier("Win32_DiskDrive", "Model")
        //           + identifier("Win32_DiskDrive", "Manufacturer")
        //           + identifier("Win32_DiskDrive", "Signature")
        //           + identifier("Win32_DiskDrive", "TotalHeads");
        //}

        //Motherboard ID
        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Model")
                   + Identifier("Win32_BaseBoard", "Manufacturer")
                   + Identifier("Win32_BaseBoard", "Name")
                   + Identifier("Win32_BaseBoard", "SerialNumber");
        }

        //Primary video controller ID
        private static string VideoId()
        {
            return Identifier("Win32_VideoController", "DriverVersion")
                   + Identifier("Win32_VideoController", "Name");
        }

        //First enabled network card ID
        private static string MacId()
        {
            return Identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }

        #endregion Original Device ID Getting Code
    }
}