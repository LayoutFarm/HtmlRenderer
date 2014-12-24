//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.WinForms
{
    public class UIPlatformWinForm : UIPlatform
    {

        public UIPlatformWinForm()
        { 
        }
        public override UITimer CreateUITimer()
        {
            return new MyUITimer();
        }

    }
}