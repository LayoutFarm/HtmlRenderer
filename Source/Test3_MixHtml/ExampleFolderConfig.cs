//Apache2, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class ExampleFolderConfig
    {

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