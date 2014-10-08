using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace TestGraphicPackage
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LayoutFarm.Drawing.WinGdiPlatform.Start();
            Application.Run(new Form1());
            LayoutFarm.Drawing.WinGdiPlatform.End();

        }
    }
}
