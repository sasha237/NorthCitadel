using System;
using System.Collections.Generic;
using System.Text;
using NerZul;
using System.Web;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace eRepConsoleVoter
{
    [assembly: SuppressIldasmAttribute()]
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
