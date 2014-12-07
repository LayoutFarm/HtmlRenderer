//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;



namespace LayoutFarm.Text
{

    class CaretRenderElement : RenderElement
    {
        //implement caret for text edit
        public CaretRenderElement(RootGraphic g, int w, int h)
            : base(g, w, h)
        {
        }
        public override void CustomDrawToThisPage(Canvas canvasPage, Rect updateArea)
        {
        }
        internal void DrawCaret(Canvas canvasPage, int x, int y)
        {
            canvasPage.FillRectangle(Color.Black, x, y, this.Width, this.Height );
        }
    }
}