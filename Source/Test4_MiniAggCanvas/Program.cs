using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Test4_AggCanvasBox
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //temp
            //TODO: fix this , 
            var platform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start();           

            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program)); 

            Application.Run(formDemoList);
        }
    }
}
