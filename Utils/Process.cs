using System;
using System.Linq;
using System.Threading;

namespace Utils
{
    public class Process
    {
        const string processLogsFile = @"ProcessUtils.txt";
        string processName;

        //public string ProcessName
        //{
        //    set { processName = value; }
        //    get { return processName; }
        //}

        public Process(string procName)
        {
            processName = procName;
        }

        public bool WaitUntilProcessRunning()
        {
            bool found = false;
            int count = 0;
            while (!found && count < 600)
            {
                var proceses = System.Diagnostics.Process.GetProcessesByName(processName);

                if (proceses.Count() > 0)
                {
                    found = true;
                }

                Thread.Sleep(1000);
                count++;
            }
            return found;
        }

        public void GetProcessesListAndKillProcess()
        {
            var proceses = System.Diagnostics.Process.GetProcessesByName(processName);

            if (proceses.Count() > 0)
            {
                foreach (System.Diagnostics.Process proces in proceses)
                {
                    KillProcess(proces);
                }
            }
            else
            {
                var fileLogs = new Logs(processLogsFile);
                fileLogs.WriteMessageToLogFile("Process " + processName + " is not running");
               // WriteLogsIntoAFile("Process " + processName + " is not running");
            }
        }

        private static void KillProcess(System.Diagnostics.Process proces)
        {
            try
            {
                proces.Kill();
            }
            catch (Exception e)
            {
                var fileLogs = new Logs(processLogsFile);
                fileLogs.WriteMessageToLogFile("Kill Process : " + e.Message);
                //WriteLogsIntoAFile(e.Message);
            }
        }

        public int LaunchProcessWithArgsAndWaitForProcessToFinish(string fileName,string arguments, int timeToWaitForProcessToFinishInMilliseconds = 0)
        {
            int result = 0;

            try
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(fileName, arguments);
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
                var fileLogs = new Logs(processLogsFile);
                fileLogs.WriteMessageToLogFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
                // WriteLogsIntoAFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
            }

            return result;
        }

        public int LaunchProcessWithoutArgs()
        {
            int result = 0;

            try
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(processName);
                
            }
            catch (Exception e)
            {
                var fileLogs = new Logs(processLogsFile);
                fileLogs.WriteMessageToLogFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
                // WriteLogsIntoAFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
            }

            return result;
        }
    }
}
