﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileMonitorSilentInstall
{
    class Program
    {
        const string logFile = @"InstallFileMonitor.txt";
        const string serverInstallerKit = "ServerInstaller.exe";
        const string installConfigFile = "silentinstallconfig.xml";
        const string processConfigName = "PostInstallServerConfig";
        const string postInstallConfigFile = @"C:\Program Files (x86)\Temasoft\FileMonitor Server\PostInstallServerConfig.exe";

        static void Main(string[] args)
        {
            switch (args.Count())
            {
                case 0:
                    Console.WriteLine("Please specify a folder path containing ServerInstaller and configuration file!");
                    break;
                case 1:
                    if (args[0] == "/?")
                    {
                        DisplayHelp();
                    }
                    else
                    {
                        RunServerInstaller(args[0]);
                    }
                    break;
                default:
                    Console.WriteLine("Please provide only one parameter!");
                    break;
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("\n\tPlease specify a folder path containing ServerInstaller and configuration file!");
            Console.WriteLine("\n\t Example:  FileMonitorSilentInstall c:\\TestPath ");
        }

        private static void RunServerInstaller(string path)
        {
            if (Directory.Exists(path))
            {
                RunServerInstallerKit(path);
            }
            else
            {
                var logMessages = "This path: " + path + " doesn't exists !";
             
                var logsFile = new Utils.Logs(logFile);
                logsFile.WriteMessageToLogFile(logMessages);
            }
        }

        private static void RunServerInstallerKit(string path)
        {
            string fullPathServerInstaller = Path.Combine(path, serverInstallerKit);
            string fullPathInstallConfig = Path.Combine(path, installConfigFile);
            string programDataInstallConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), installConfigFile);

            if (File.Exists(fullPathServerInstaller) && File.Exists(fullPathInstallConfig))
            {
                PerformInstallerCommands(fullPathServerInstaller, fullPathInstallConfig, programDataInstallConfig);
            }
            else
            {
                var errorMessage = "Some required files are missing";
               
                var logsFile = new Utils.Logs(logFile);
                logsFile.WriteMessageToLogFile(errorMessage);
            }
        }

        private static void PerformInstallerCommands(string fullPathServerInstaller, string fullPathInstallConfig, string programDataInstallConfig)
        {
            var logsFile = new Utils.Logs(logFile);

            if (File.Exists(programDataInstallConfig))
            {
                try
                {
                    File.Delete(programDataInstallConfig);
                }
                catch (Exception e)
                {
                    logsFile.WriteMessageToLogFile("Exception when try to delete silentconfig file  : " + e.Message);
                }
            }
            Process.Start(fullPathServerInstaller, "/i /q");
            Console.WriteLine("Installing FileMonitor Server...");
            var configProcess = new Utils.Process(processConfigName);
           
            bool proceessIsRunning = configProcess.WaitUntilProcessRunning();
            if (proceessIsRunning)
            {
                configProcess.GetProcessesListAndKillProcess();
                File.Copy(fullPathInstallConfig, programDataInstallConfig, true);

                Console.WriteLine("Configure FileMonitor Server...");
                var result = configProcess.LaunchProcessWithArgsAndWaitForProcessToFinish(postInstallConfigFile, "/install", 60000);
                if (result != 0)
                {
                    var errorMessage = "An error was occured on PostInstallConfig or timed out!!";
                    logsFile.WriteMessageToLogFile(errorMessage);
                }
                else
                {
                    var Message = "Application was succesfully installed !!!";
                    Console.WriteLine(Message);
                    logsFile.WriteMessageToLogFile(Message);
                }
            }
            else
            {
                var errorMessage = "Something was wrong or timed out!!";
                logsFile.WriteMessageToLogFile(errorMessage);
            }
        }

    }
}
