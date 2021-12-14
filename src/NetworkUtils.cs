#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class NetworkUtils
    {
        public static IPAddress GetLocalIp(Socket activeSocket)
        {
            activeSocket.ThrowIfNull(nameof(activeSocket));

            if (!(activeSocket.LocalEndPoint is IPEndPoint ipEndPoint) || Equals(ipEndPoint.Address, IPAddress.Any))
                throw new InvalidOperationException("Socket not connected");

            return ipEndPoint.Address;
        }

        public static IPAddress GetIpAddress(uint ipAddress)
        {
            var addressBytes = BitConverter.GetBytes(ipAddress);
            Array.Reverse(addressBytes);

            return new IPAddress(addressBytes);
        }

        public static uint GetIpAddress(IPAddress ipAddress)
        {
            ipAddress.ThrowIfNull(nameof(ipAddress));

            var addressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(addressBytes);

            return BitConverter.ToUInt32(addressBytes, 0);
        }

        public static uint NetworkToHostOrder(uint input)
        {
            return (uint) IPAddress.NetworkToHostOrder((int) input);
        }

        public static ulong NetworkToHostOrder(ulong input)
        {
            return (ulong) IPAddress.NetworkToHostOrder((long) input);
        }

        public static ushort NetworkToHostOrder(ushort input)
        {
            return (ushort) IPAddress.NetworkToHostOrder((short) input);
        }

        public static bool TryParseIpEndPoint(string stringValue, out IPEndPoint? endPoint)
        {
            stringValue.ThrowIfNullOrWhiteSpace(nameof(stringValue));

            var endpointParts = stringValue.Split(':');
            if (endpointParts.Length != 2)
            {
                endPoint = null;
                return false;
            }

            if (!IPAddress.TryParse(endpointParts[0], out var address))
            {
                endPoint = null;
                return false;
            }

            if (!ushort.TryParse(endpointParts[1], out var port))
            {
                endPoint = null;
                return false;
            }

            endPoint = new IPEndPoint(address, port);
            return true;
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsConnectedToInternet()
        {
            try
            {
                return InternetGetConnectedState(out _, 0);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}