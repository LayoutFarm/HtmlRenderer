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
        GraphicsPlatform graphicsPlatform;
        public UIPlatformWinForm(GraphicsPlatform graphicsPlatform)
        {
            this.graphicsPlatform = graphicsPlatform;
        }
        public override UITimer CreateUITimer()
        {
            return new MyUITimer();
        }
        public override GraphicsPlatform GraphicsPlatform
        {
            get { return graphicsPlatform; }
        }
    }
}