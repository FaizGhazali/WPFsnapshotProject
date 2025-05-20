using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace SnapShotHelper
{
    public class RegistryConfigurator
    {
        [SupportedOSPlatform("windows")]
        public static void SaveItemToRegistry(string projectName, string folder,
            Dictionary<string, string?> stringitems,
            Dictionary<string, bool> boolitems)
        {
            string registryKeyPath = string.IsNullOrEmpty(folder)
             ? @$"SOFTWARE\{projectName}"
             : @$"SOFTWARE\{projectName}\{folder}";



            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryKeyPath))
            {
                if (key != null)
                {
                    // Save string items
                    if (stringitems != null)
                    {
                        foreach (var item in stringitems)
                        {
                            key.SetValue(item.Key, item.Value ?? "", RegistryValueKind.String);
                        }
                    }

                    // Save boolean items as DWORD (1 for true, 0 for false)
                    if (boolitems != null)
                    {
                        foreach (var item in boolitems)
                        {
                            key.SetValue(item.Key, item.Value ? 1 : 0, RegistryValueKind.DWord);
                        }
                    }
                }

            }
        }




        [SupportedOSPlatform("windows")]
        public static Dictionary<string, object> GetAllRegistry(string projectName, string folder)
        {
            string registryKeyPath = string.IsNullOrEmpty(folder)
             ? @$"SOFTWARE\{projectName}"
             : @$"SOFTWARE\{projectName}\{folder}";

            Dictionary<string, object> registry = new Dictionary<string, object>();

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKeyPath))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            object value = key.GetValue(valueName);
                            registry[valueName] = value; // Store the value as 'object' to support various types
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Registry key '{registryKeyPath}' not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing registry: {ex.Message}");
            }

            return registry;
        }
    }
}
