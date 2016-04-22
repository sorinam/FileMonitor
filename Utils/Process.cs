using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Utils
{
    public class ProcessUtils
    {
        string processName;

        //public string ProcessName
        //{
        //    set { processName = value; }
        //    get { return processName; }
        //}

        public ProcessUtils(string procName)
        {
            processName = procName;
        }

        public bool WaitUntilAProcessRunning()
        {
            bool found = false;
            int count = 0;
            while (!found && count < 600)
            {
                var proceses = Process.GetProcessesByName(processName);

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
               // WriteLogsIntoAFile("Process " + processName + " is not running");
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
                //WriteLogsIntoAFile(e.Message);
            }
        }

        private static int LaunchProcessAndWaitForProcessToFinish(string fileName, int timeToWaitForProcessToFinishInMilliseconds = 0)
        {
            int result = 0;

            try
            {
                Process p = Process.Start(fileName, "/install");
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
               // WriteLogsIntoAFile("LaunchProcessAndWaitForProcessToFinish : " + e.Message);
            }

            return result;
        }
    }
}
