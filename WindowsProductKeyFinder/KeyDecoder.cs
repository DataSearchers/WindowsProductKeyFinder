using System;
using Microsoft.Win32;

namespace WindowsProductKeyFinder
{
    public static class KeyDecoder
    {
        public static string Kdec()
        {
            var lKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            var registryHive = lKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")?.GetValue("digitalProductId");

            if (registryHive == null)
                return "Fail.";

            var sKey = (byte[])registryHive;
            lKey.Close();
            return IdFromKey(sKey);
        }

        public static string IdFromKey(byte[] sKey)
        {
            var vkey = getKeyData(sKey);
            return vkey;
        }

        public static string getKeyData(byte[] sKey)
        {
            var key = string.Empty;
            const int keyOffset = 52;
            var windprd = (byte)((sKey[66] / 6) & 1);
            sKey[66] = (byte)((sKey[66] & 0xf7) | (windprd & 2) * 4);

            const string digits = "BCDFGHJKMPQRTVWXY2346789";
            var last = 0;
            for (var i = 24; i >= 0; i--)
            {
                var current = 0;
                for (var j = 14; j >= 0; j--)
                {
                    current = current * 256;
                    current = sKey[j + keyOffset] + current;
                    sKey[j + keyOffset] = (byte)(current / 24);
                    current = current % 24;
                    last = current;
                }
                key = digits[current] + key;
            }
            var k1 = key.Substring(1, last);
            var k2 = key.Substring(last + 1, key.Length - (last + 1));
            key = k1 + "N" + k2;

            for(var i = 5; i < key.Length; i += 6)
            {
                key = key.Insert(i, "-");
            }
            return key;
        }
    }
}
