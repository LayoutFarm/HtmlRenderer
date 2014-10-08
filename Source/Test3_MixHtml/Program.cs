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


             
            LayoutFarm.Drawing.WinGdiPortal.Start();


            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo =
                LayoutFarm.Drawing.CurrentGraphicPlatform.CreateTexFontInfo(
                    new System.Drawing.Font("tahoma", 10));

            var formDemo = new LayoutFarm.Dev.FormDemoList();
            formDemo.LoadDemoList(typeof(Program));


            Application.Run(formDemo);

            LayoutFarm.Drawing.WinGdiPortal.End();

        }
    }
}
