using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.Ease;

namespace Test5_Ease
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            EaseHost.StartGraphicsHost();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
