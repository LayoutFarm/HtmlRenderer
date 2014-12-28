//2014 Apache2, WinterDev
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



            var platform = LayoutFarm.Drawing.MyWinGdiPortal.Start();


            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo = platform.GetFont("tahoma", 10);

            var formDemo = new LayoutFarm.Dev.FormDemoList(platform);
            formDemo.LoadDemoList(typeof(Program));


            Application.Run(formDemo);

            LayoutFarm.Drawing.MyWinGdiPortal.End();

        }
    }
}
