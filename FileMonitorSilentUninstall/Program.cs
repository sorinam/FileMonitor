using FileMonitorSilentUninstall;
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
        const string logFile = @"UninstallFileMonitor.txt";

        static void Main(string[] args)
        {
            var logsFile = new Logs(logFile);

            var uninstallStringServer = GetUninstallStringOfApplication(productServerName);
            var uninstallStringAgent = GetUninstallStringOfApplication(productAgentName);
            string message = "";
            
            if ((uninstallStringAgent == "") && (uninstallStringServer == ""))
            {
                message = "FileMonitor is not installed on this computer!";
                logsFile.WriteMessageToLogFile(message);
                Console.WriteLine(message);
                Thread.Sleep(1000);
            }
            else
            {
                if (uninstallStringAgent != "")
                {
                    message = String.Format("Application '{0}' will be uninstalled !", productAgentName);
                    logsFile.WriteMessageToLogFile(message);
                    Console.WriteLine(message);
                    RunUninstallCommand(uninstallStringAgent);
                }
                if (uninstallStringServer != "")
                {
                    message = String.Format("Application '{0}' will be uninstalled !", productServerName);
                    logsFile.WriteMessageToLogFile(message);
                    Console.WriteLine(message);
                    RunUninstallCommand(uninstallStringServer);
                }
            }
            
        }
    
        private static void RunUninstallCommand(string uninstallString)
        {
            var logsFile = new Logs(logFile);
            string arguments = " /quiet /norestart ";
            string GUID = uninstallString.Substring(14);
            string parameters = "/X " + GUID + arguments;
            string message = "";
            Console.WriteLine(" Uninstall command  is: MsiExec.exe{0}\n", parameters);
            try
            {
                var process = Process.Start("MsiExec.exe", parameters);
                process.WaitForExit();
                message = String.Format("Exit Code MsiExec: {0} ", process.ExitCode);
            }
            catch (Exception e)
            {
                message=String.Format("Exception MsiExec Message: {0} ", e.Message);
            }
            Console.WriteLine(message);
            logsFile.WriteMessageToLogFile(message);
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