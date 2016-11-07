//Apache2, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class ExampleFolderConfig
    {
        public static void InitIcuData()
        {
            string icu_dataFile = @"D:\WImageTest\icudt57l\icudt57l.dat";
            PixelFarm.Drawing.Text.NativeTextBreaker.SetICUDataFile(icu_dataFile);
        }
        public static string GetCheckFolder()
        {
#if DEBUG
            string checkFolder = "\\Source\\Test3_MixHtml\\bin\\Debug";
#else
            string checkFolder = "\\Source\\Test3_MixHtml\\bin\\Release";
#endif
            return checkFolder;

        }
    }
}