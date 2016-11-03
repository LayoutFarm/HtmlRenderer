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
            EaseHost.LibEspr = @"D:\projects\HTML-Renderer\Source\Deps\ess_natives\libespr.dll";
            EaseHost.LoadLibEspr = true;
            EaseHost.IcuDataFile = @"D:\WImageTest\icudt57l\icudt57l.dat";
            EaseHostInitReport report = EaseHost.Check();
            if (report.HasSomeError)
            {
                throw new NotSupportedException();
            }
            EaseHost.Init();
             
            //----------------------------------------------------------------------------
            EaseHost.StartGraphicsHost();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }
    }
}
