using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Narod
{
    internal static class RegistryHandler
    {
        public static string? safeGetRegistryKey(string keyName, string regPath)
        {
            object regKeyObj = Registry.GetValue(regPath, keyName, null);
            if (regKeyObj != null)
            {
                return regKeyObj.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
