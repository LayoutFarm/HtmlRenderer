//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Forms;

namespace LayoutFarm.UI.WinNeutral
{
    public class Control
    {
        List<Control> _controls = new List<Control>();
        protected virtual void OnLoad(EventArgs e)
        {
        }
        public bool Visible { get; set; }
        public void Focus() { }
        public void Show() { }
        public void Hide() { }
        public List<Control> Controls
        {
            get
            {
                return this._controls;
            }
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
    }
    public class UserControl : Control
    {

    }
    public class Form : Control
    {
        public void Close() { }
    }
}