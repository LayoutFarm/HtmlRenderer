// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{

    public class CustomTextRun : RenderElement
    {

        char[] textBuffer;
     
#if DEBUG
        public bool dbugBreak;
#endif
        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {

        }
        public string Text
        {
            get { return new string(this.textBuffer); }
            set
            {
                if (value == null)
                {
                    this.textBuffer = null;
                }
                else
                {
                    this.textBuffer = value.ToCharArray(); 
                }
            }
        }
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            if (this.textBuffer != null)
            {   
                canvas.DrawText(this.textBuffer, this.X, this.Y);
            }
        }

    }



}