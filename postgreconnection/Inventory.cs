using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace postgreconnection
{
    /*
    note: on windows 10 cortana populates add/remove programs list by searching for .exe on all hdd's
    this class takes installed programs from registry - the list should be identical to add/remove on any windows OS which is not win10
    */
    static class Inventory
    {
        
        const string InventoryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        [DllImport("msi.dll")]
        static private extern Int32 MsiGetProductInfo(string guid, string property, [Out] StringBuilder valueBuf, ref Int32 len);

        static public List<string> GetLocalInventory()
        {
            List<string> LocalInventory = new List<string>();

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(InventoryKey))
            {
                foreach (string x in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(x))
                    {
                        if (IsProgramVisible(subkey))
                            LocalInventory.Add((string)subkey.GetValue("DisplayName"));
                    }                        
                }
            }
            return LocalInventory;
        }

        static private bool IsProgramVisible(RegistryKey subkey)
        {
            string dn = (string)subkey.GetValue("DisplayName");
            var sc = subkey.GetValue("SystemComponent"); //REG_DWORD - cant cast to string. Rest of properties is REG_SZ
            string pdn = (string)subkey.GetValue("ParentDisplayName");
            string rt = (string)subkey.GetValue("ReleaseType");
            var wi = subkey.GetValue("WindowsInstaller"); //REG_DWORD

            if (wi != null) //windows installer entries (those with {fDSBDuDF-SDFBEDS} key names)
            {
                //strip guid from path
                string guid = subkey.Name;
                int i = guid.LastIndexOf('\\');
                guid = guid.Substring(i+1);

                Int32 len = 512;
                StringBuilder builder = new StringBuilder();

                Int32 code = MsiGetProductInfo(guid, "VersionString", builder, ref len);

                if (code == 1605) //error unknown_product -> program was uninstalled yet registry entry still exists
                    return false;
                else
                    return true;
            }
            else //other entries with non-guid names
            {
                return !string.IsNullOrEmpty(dn) && sc == null && string.IsNullOrEmpty(pdn) && string.IsNullOrEmpty(rt);
            }

        }


    }
}
