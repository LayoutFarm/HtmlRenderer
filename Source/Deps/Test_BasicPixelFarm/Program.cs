using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Test_BasicPixelFarm
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;
        [STAThread]
        static void Main()
        {

             //ExampleFolderConfig.InitIcuData();
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //temp
            //TODO: fix this , 
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyNativeTextBreaker();

            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));
            //LoadHtmlSamples(formDemoList.SamplesTreeView);
            Application.Run(formDemoList);
        }
        
    }
}
