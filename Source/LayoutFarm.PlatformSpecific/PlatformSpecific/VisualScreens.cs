//2014 Apache2, WinterDev
using System;
using System.Drawing;
namespace LayoutFarm.Presentation
{
    public static class VisualScreen
    {
        public static Rectangle GetPrimaryScreenWorkingArea()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
        }
    }
}