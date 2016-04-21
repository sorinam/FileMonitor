using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileMonitorSilentInstall
{
    class Program
    {
        const string logFile = @"SilentInstall.txt";
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
                    RunServerInstaller(args[0]);
                    break;
                default:
                    Console.WriteLine("Please provide only one parameter!");
                    break;
            }
        }

        private static void RunServerInstaller(string path)
        {
            if (Directory.Exists(path))
            {
                RunServerInstallerKit(path);
            }
            else
            {
                var logMessages = "This path: " + path + " doesn't exists";
                WriteLogsIntoAFile(logMessages);
            }
        }

        private static void RunServerInstallerKit(string path)
        {
            string fullPathServerInstaller = Path.Combine(path, serverInstallerKit);
            string fullPathInstallConfig = Path.Combine(path, installConfigFile);
            string programDataInstallConfig = Path.Combine(@"c:\ProgramData", installConfigFile);

            if (System.IO.File.Exists(fullPathServerInstaller) && System.IO.File.Exists(fullPathInstallConfig))
            {
                PerformCommands(fullPathServerInstaller, fullPathInstallConfig, programDataInstallConfig);
            }
            else
            {
                var errorMessage = "Some needed file are missing";
                WriteLogsIntoAFile(errorMessage);
            }
        }

        private static void PerformCommands(string fullPathServerInstaller, string fullPathInstallConfig, string programDataInstallConfig)
        {
            if (File.Exists(programDataInstallConfig))
            {
                File.Delete(programDataInstallConfig);
            }

            Process.Start(fullPathServerInstaller, "/i /q");

            bool proceessIsRunning = WaitUntilConfigProcessRunning();

            if (proceessIsRunning)
            {
                GetAllProcessesAndKillConfigProcess(processConfigName);
                File.Copy(fullPathInstallConfig, programDataInstallConfig, true);

                Thread.Sleep(150000);

                //Process.Start(postInstallConfigFile);
                var result = LaunchProcessAndWaitForProcessToFinish(postInstallConfigFile, 60000);
                if (result != 0)
                {
                    var errorMessage = "An error was occured on PostInstallConfig or timed out!!";
                    WriteLogsIntoAFile(errorMessage);
                }
            }
            else
            {
                var errorMessage = "Something was wrong or timed out!!";
                WriteLogsIntoAFile(errorMessage);
            }
        }

        private static bool WaitUntilConfigProcessRunning()
        {
            bool found = false;
            int count = 0;
            while (!found && count < 100)
            {
                var proceses = Process.GetProcessesByName(processConfigName);

                if (proceses.Count() > 0)
                {
                    found = true;
                }

                Thread.Sleep(1000);
                count++;
            }

            return found;
        }

        private static void GetAllProcessesAndKillConfigProcess(string processName)
        {
            var proceses = Process.GetProcessesByName(processName);

            if (proceses.Count() > 0)
            {
                foreach (Process proces in proceses)
                {
                    KillProcess(proces);
                }
            }
            else
            {
                WriteLogsIntoAFile("Process " + processName + " is not running");
            }
        }

        private static void KillProcess(Process proces)
        {
            try
            {
                proces.Kill();
            }
            catch (Exception e)
            {
                WriteLogsIntoAFile(e.Message);
            }
        }

        private static int LaunchProcessAndWaitForProcessToFinish(string commandLine, int timeToWaitForProcessToFinishInMilliseconds = 0)
        {
            int result = 0;

            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo(commandLine);
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                //startinfo.UseShellExecute = true;
                Process p = Process.Start(commandLine, "/install");
                if (timeToWaitForProcessToFinishInMilliseconds == 0)
                {
                    p.WaitForExit();
                }
                else
                {
                    p.WaitForExit(timeToWaitForProcessToFinishInMilliseconds);
                }

                if (p.HasExited)
                {
                    result = p.ExitCode;
                }
            }
            catch (Exception e)
            {
                WriteLogsIntoAFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
            }

            return result;
        }

        private static void WriteLogsIntoAFile(string LogMessage)
        {
            if (System.IO.Directory.Exists(logFile))
            {
                File.AppendAllText(logFile, LogMessage);
            }
            else
            {
                System.IO.File.WriteAllText(logFile, LogMessage);
            }
        }
    }
}
