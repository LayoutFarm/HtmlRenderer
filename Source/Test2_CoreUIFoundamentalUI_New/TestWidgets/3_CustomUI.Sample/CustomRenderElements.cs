// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class CustomRenderBox : RenderBoxBase
    {

#if DEBUG
        public bool dbugBreak;
#endif
        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
            
        }
        public override void ClearAllChildren()
        {
        }
        public Color BackColor
        {
            get;
            set;
        }
        protected override void DrawContent(Canvas canvas, Rect updateArea)
        {
            //sample bg   
            canvas.FillRectangle(BackColor, updateArea._left, updateArea._top, updateArea.Width, updateArea.Height);
            if (this.Layers != null)
            {
                this.Layers.LayersDrawContent(canvas, updateArea);
            }
#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
    }



}