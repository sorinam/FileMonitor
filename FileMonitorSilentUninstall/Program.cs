using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;

namespace FileMonitorFullSilentUninstall
{
    class Program
    {
        const string productServerName = "Temasoft FileMonitor Server";
        const string productAgentName = "Temasoft FileMonitor Agent";

        static void Main(string[] args)
        {

            var uninstallStringServer = GetUninstallStringOfApplication(productServerName);
            var uninstallStringAgent = GetUninstallStringOfApplication(productAgentName);

            if ((uninstallStringAgent == "") && (uninstallStringServer == ""))
            {
                Console.WriteLine("FileMonitor is not installed on this computer!");
                Thread.Sleep(1000);
            }
            else
            {
                if (uninstallStringAgent != "")
                {
                    Console.WriteLine("Application '{0}' will be uninstalled !", productAgentName);
                    RunUninstallCommand(uninstallStringAgent);
                }
                if (uninstallStringServer != "")
                {
                    Console.WriteLine("Application '{0}' will be uninstalled !", productServerName);
                    RunUninstallCommand(uninstallStringServer);
                }
            }
        }

        private static void RunUninstallCommand(string uninstallString)
        {
            string arguments = " /quiet /norestart ";
            string GUID = uninstallString.Substring(14);
            string parameters = "/X " + GUID + arguments;
            Console.WriteLine("\t Uninstall command  is: MsiExec.exe{0}\n", parameters);
            try
            {
                var process = Process.Start("MsiExec.exe", parameters);
                process.WaitForExit();
                Console.WriteLine("Exit Code: {0} ", process.ExitCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Message: {0} ", e.Message);
            }
        }

        public static string GetUninstallStringOfApplication(string productName)
        {
            string result = "";
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey subKey = registryKey.OpenSubKey(skName))
                    {
                        try
                        {
                            if (subKey.GetValue("DisplayName").ToString() == productName)
                            {
                                result = subKey.GetValue("UninstallString").ToString();
                                return result;
                            };
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                return result;
            }
        }
    }
}