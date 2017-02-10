//Apache2, 2014-2017, WinterDev
using System;
using PixelFarm.Forms;

namespace LayoutFarm.UI.WinNeutral
{
    public class Control
    {
        protected virtual void OnLoad(EventArgs e)
        {
        }
        public bool Visible { get; set; }
        public void Focus() { }
        public void Show() { }
        public void Hide() { }
    }
    public class UserControl : Control
    {

    }
    public class Form : Control
    {
        public void Close() { }
    }
}