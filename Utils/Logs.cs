using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Logs
    {
        string logFile;
        public Logs(string fileName)
        {
            logFile = fileName;
        }
        
        public void WriteIntoALogFile(string LogMessage)
        {
            if (Directory.Exists(logFile))
            {
                File.AppendAllText(logFile, LogMessage);
            }
            else
            {
                File.WriteAllText(logFile, LogMessage);
            }
        }
    }
}
