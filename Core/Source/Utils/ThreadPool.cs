using System;
namespace NerZul.Core.Utils.Bicycles
{
    public static class ThreadPool
    {
        private struct Config
        {
            public System.Threading.ParameterizedThreadStart Routine;
            public object Argument;
            public bool ReportErrors;
            public Config(System.Threading.ParameterizedThreadStart routine,
                          object argument, bool reporterrors)
            {
                Routine = routine;
                Argument = argument;
                ReportErrors = reporterrors;
            }
        }

        private static void ThreadProc(object arg)
        {
            Config cfg = (Config)arg;
            try
            {
                cfg.Routine.Invoke(cfg.Argument);
            }
            catch (Exception e)
            {
                if (cfg.ReportErrors)
                {
                    ConsoleLog.WriteLine("Exception: " + e.GetType().ToString() + ": " + e.Message +
                                        "\n" + e.StackTrace);
                }
            }
        }
        public static void ShowDlg(object arg)
        {
            System.Threading.Thread[] pool = arg as System.Threading.Thread[];
            TerminationForm dlg = new TerminationForm();
            dlg.pool = pool;
            dlg.ShowDialog();
        }
        public static void ExecInPool(System.Threading.ParameterizedThreadStart Routine,
                                      System.Collections.IEnumerable Args,
                                      int PoolSize, bool ReportErrors, bool bShowDialog)
        {
            System.Threading.Thread[] pool = new System.Threading.Thread[PoolSize];
            System.Threading.Thread dlgThread = null;

            if (bShowDialog)
            {
                dlgThread = new System.Threading.Thread(ShowDlg);
                dlgThread.Start(pool);
            }

            foreach (var botnfo in Args)
            {
                while (true)
                {
                    bool found = false;
                    for (int i = 0; i < pool.Length; i++)
                    {
                        if ((pool[i] == null) || (pool[i].IsAlive == false))
                        {
                            Config cfg = new Config(Routine, botnfo, ReportErrors);
                            pool[i] = new System.Threading.Thread(ThreadProc);
                            pool[i].Start(cfg);
                            found = true;
                            ConsoleLog.WriteLine("ThreadPool. ThreadCount: " + System.Diagnostics.Process.GetCurrentProcess().Threads.Count.ToString());
                            break;
                        }
                    }
                    if (found) break;
                    System.Threading.Thread.Sleep(1000);
                }
            }

            //Ждём завершения потоков
            while (true)
            {
                bool found = false;
                for (int i = 0; i < pool.Length; i++)
                {
                    if ((pool[i] != null) && (pool[i].IsAlive))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) break;
               
                if ((bShowDialog) && (!dlgThread.IsAlive))
                {
                    for (int i = 0; i < pool.Length; i++)
                    {
                        if (pool[i] != null)
                            pool[i].Abort();
                    }
                }

                System.Threading.Thread.Sleep(1000);
            }

            if ((bShowDialog) && (dlgThread.IsAlive))
                dlgThread.Abort();
        }

        public static void ExecInPool(System.Threading.ParameterizedThreadStart Routine,
                                      System.Collections.IEnumerable Args, int PoolSize, bool bShowDialog)
        {
            ExecInPool(Routine, Args, PoolSize, true, bShowDialog);
        }
    }
}

