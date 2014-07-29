using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LayoutFarm.Presentation
{
    public static class PlatformMessageBox
    {
        public static void Show(string text)
        {
            MessageBox.Show(text);
        }
    }
}