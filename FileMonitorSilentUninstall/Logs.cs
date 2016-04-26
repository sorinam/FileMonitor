using System;
using System.IO;

namespace FileMonitorSilentUninstall
{
    class Logs
    {
        string logFile;
        public Logs(string fileName)
        {
            logFile = fileName;
        }

        public void WriteMessageToLogFile(string LogMessage)
        {
            var log = "\r\n" + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString() + " " + LogMessage;

            using (StreamWriter w = File.AppendText(logFile))
            {
                w.Write(log);
            }
        }
    }
}
