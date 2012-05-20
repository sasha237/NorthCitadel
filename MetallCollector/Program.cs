using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Management;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace MetallCollector
{
    [assembly: SuppressIldasmAttribute()]
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AutoForm());
        }
    }
}
