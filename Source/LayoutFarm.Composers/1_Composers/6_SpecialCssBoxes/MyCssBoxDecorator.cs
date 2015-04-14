//BSD 2014-2015 ,WinterDev 
using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm.HtmlBoxes
{
    class MyCssBoxDescorator : CssBoxDecorator
    {

        public MyCssBoxDescorator(CssBox targetBox)
        {
            this.TargetBox = targetBox;
        }
        
        public override void DrawTopDecoration(PaintVisitor p)
        {
            
        }
      
        public CssBox TargetBox
        {
            get;
            private set;
        }
    }

}