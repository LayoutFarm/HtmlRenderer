//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.WinForm
{
    public class UIPlatformWinForm : UIPlatform
    {
        public override UITimer CreateUITimer()
        {                
            return new MyUITimer();
        }
    }
}