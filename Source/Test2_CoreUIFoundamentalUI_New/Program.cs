//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace TestGraphicPackage2
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            var platform = PixelFarm.Drawing.MyWinGdiPortal.Start();

            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo = platform.GetFont("tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);

            var formDemo = new LayoutFarm.Dev.FormDemoList();
            formDemo.LoadDemoList(typeof(Program));


            Application.Run(formDemo);
            PixelFarm.Drawing.MyWinGdiPortal.End();

        }
    }
}
