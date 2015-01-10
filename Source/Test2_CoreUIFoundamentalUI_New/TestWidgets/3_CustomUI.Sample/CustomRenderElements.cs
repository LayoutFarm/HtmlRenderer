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

    public class CustomRenderBox : RenderBoxes.RenderBoxBase
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
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            //sample bg   
            canvas.FillRectangle(BackColor, updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height);
            if (this.MyLayers != null)
            {
                this.MyLayers.LayersDrawContent(canvas, updateArea);
            }
#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
        public VisualLayerCollection Layers
        {
            get { return this.MyLayers; }
            set { this.MyLayers = value; }
        }
    }



}