﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (uninstallStringServer != "")
            {
                Console.WriteLine("Application '{0}' is installed .", productServerName);
                Console.WriteLine("Unnstall String is: {0}", uninstallStringServer);
                RunUninstallCommand(uninstallStringServer);
            }
            else

                if (uninstallStringAgent != "")
            {
                Console.WriteLine("Application '{0}' is installed .", productAgentName);
                Console.WriteLine("Unnstall String is: {0}", uninstallStringAgent);
                RunUninstallCommand(uninstallStringAgent);
            }
            else
            {
                Console.WriteLine("FileMonitor is not installed!");
            }
            Console.ReadKey();
        }

        private static void RunUninstallCommand(string uninstallString)
        {
            string arguments = " /quiet /norestart ";
            //Console.WriteLine("Arguments:{0}",uninstallString+ arguments);
            string parameters = uninstallString.Substring(11) + arguments;
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