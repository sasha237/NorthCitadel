using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Threading;

namespace NerZul.Core.Utils
{
    public static class ConsoleLog
    {
        private static object locker = new object();

        const string FileConsoleLog = "ConsoleLog.txt";
        const string ThreadAbortException = "Thread was being aborted";

        static ConsoleLog()
        {
            File.Delete(FileConsoleLog);
        }

        public static void WriteLine(String message, String fileName, bool printToScreen)
        {
            try
            {
                string logMess =
                    Thread.CurrentThread.ManagedThreadId.ToString() + ": " +
                    message +
                    Environment.NewLine;

                lock (locker)
                {
                    if (printToScreen)
                        Console.WriteLine(logMess);

                    var threadWriter = new StreamWriter(
                        Path.GetDirectoryName(Application.ExecutablePath) + "\\" + fileName,
                        true);
                    threadWriter.WriteLine(logMess);
                    threadWriter.Close();
                }
            }

            catch (Exception)
            {
            }
        }

        public static void WriteLine(String message)
        {
            if (message.IndexOf(ThreadAbortException) != -1) return;

            ConsoleLog.WriteLine(message, FileConsoleLog, true);
        }

        public static void WriteLine(String message, String fileName)
        {
            ConsoleLog.WriteLine(message, fileName, false);
        }
    }
}
