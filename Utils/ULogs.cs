using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ULogs
    {
        string logFile;
        public ULogs(string fileName)
        {
            logFile = fileName;
        }
        
        public void WriteMessageToLogFile(string LogMessage)
        {
            var log = "\r\n"+DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString() + " " + LogMessage;
           
            using (StreamWriter w = File.AppendText(logFile))
            {
                w.Write(log);
            }
        }
    }
}
