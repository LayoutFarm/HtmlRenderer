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
            var startPars = new LayoutFarm.UI.GdiPlus.MyWinGdiPortalSetupParameters();
            var platform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start(startPars);
            Application.Run(new Form1(platform));
            LayoutFarm.UI.GdiPlus.MyWinGdiPortal.End();
        }
    }
}
