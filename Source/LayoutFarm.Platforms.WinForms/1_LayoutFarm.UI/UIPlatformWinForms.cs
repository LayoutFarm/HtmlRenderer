// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;

namespace LayoutFarm.UI 
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