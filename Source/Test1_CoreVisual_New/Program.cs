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

            var platform = PixelFarm.Drawing.MyWinGdiPortal.Start();
            Application.Run(new Form1(platform));
            PixelFarm.Drawing.MyWinGdiPortal.End();

        }
    }
}
